using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Obtain_25_Cat_Images.Data;
using Obtain_25_Cat_Images.DTOs;
using Obtain_25_Cat_Images.Helpers;
using Obtain_25_Cat_Images.Interfaces;
using Obtain_25_Cat_Images.Models.Entities;
using System;

namespace Obtain_25_Cat_Images.Services {
    public class CatService(AppDbContext context, IMapper mapper, HttpClient httpClient) : ICatService {
        
        public async Task<Result<List<CatResponseDTO>>> FetchAndSaveCatsAsync() {
            try {
                string url = "images/search?limit=25&has_breeds=1"; 
                var response =  await httpClient.GetFromJsonAsync<List<CatApiResponseDTO>>(url);

                if (response == null || response.Count == 0) {
                    Console.WriteLine($"[Error] Could not get data from {url}");
                    return Result.Fail("There was an error fetching cats.");
                }

                var existingTags = await context.Tags.ToListAsync();
                var tagsDict = existingTags.ToDictionary(t => t.Name.ToLower(), t => t);

                var catEntityList = new List<CatEntity>();

                foreach (var cat in response) {
                    if (context.Cats.Any(c => c.CatId == cat.Id)) continue;

                    var catEntity = new CatEntity() {
                        CatId = cat.Id,
                        Image = cat.Url,
                        Width = cat.Width,
                        Height = cat.Height,
                        CatTags = []
                    };

                    var tags = cat.Breeds
                        .SelectMany(p => Parser.ParseTemperament(p.Temperament))
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .Distinct()
                        .ToList();

                    foreach (var name in tags) {
                        var key = name.ToLowerInvariant();
                       
                        if (!tagsDict.TryGetValue(key, out var tagEntity)) {
                            tagEntity = new TagEntity() { Name = name };
                            context.Tags.Add(tagEntity);
                            tagsDict[key] = tagEntity;
                        }
                        context.CatTags.Add(new CatTagEntity() { Cat = catEntity, Tag = tagEntity }); 
                    }

                    context.Cats.Add(catEntity);
                    catEntityList.Add(catEntity);
                }
                await context.SaveChangesAsync();
                return Result.Ok(mapper.Map<List<CatResponseDTO>>(catEntityList));
            }
            catch (Exception ex) {
                Console.WriteLine($"[Error] {nameof(FetchAndSaveCatsAsync)} -> Exception thrown {ex.Message}");
                return Result.Fail("An error occured please check you internet connection");
            }

        }

        public async Task<Result<CatResponseDTO>> GetCatByIdAsync(int id) {
            try {

                var cat = await context.Cats
                    .Include(ct => ct.CatTags)
                    .ThenInclude(t => t.Tag)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cat == null) {
                    Console.WriteLine($"[Error] Cat with Id:{id} not found");
                    return Result.Fail("Cat not found");
                }

                return Result.Ok(mapper.Map<CatResponseDTO>(cat));
            }
            catch (Exception ex) {
                Console.WriteLine($"[Error] {nameof(GetCatByIdAsync)} -> Exception thrown {ex.Message}");
                return Result.Fail("An error occured please check you internet connection");
            }
        }

        public async Task<Result<List<CatResponseDTO>>> GetCatsAsync(string? tag = null, int page = 1, int pageSize = 10) {

            try {
                var query = context.Cats
                    .Include(ct => ct.CatTags)
                    .ThenInclude(t => t.Tag)
                    .AsQueryable();

                if (tag != null) {
                    query = query.Where(ct => ct.CatTags.Any(t => t.Tag.Name == tag));
                }

                var cats = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (cats.Count <= 0) {
                    Console.WriteLine("[Error] There are no cats to return.");
                    return Result.Fail("Cats not found");
                }

                return Result.Ok(mapper.Map<List<CatResponseDTO>>(cats));
            }
            catch (Exception ex) {
                Console.WriteLine($"[Error] {nameof(GetCatsAsync)} -> Exception thrown {ex.Message}");
                return Result.Fail("An error occured please check you internet connection");
            }
        }
    }
}
