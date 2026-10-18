// GolBet.Repositories/Implementations/MatchRepository.cs
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using GolBet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Implementations;

public class MatchRepository : GenericRepository<Match>, IMatchRepository
{
    public MatchRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Match>> GetAllWithTeamsAsync(
        MatchStatus? status = null,
        string? search = null)
    {
        var query = _dbSet
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.IsActive)
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();

            query = query.Where(m =>
                m.HomeTeam.Name.ToLower().Contains(searchTerm) ||
                m.AwayTeam.Name.ToLower().Contains(searchTerm));
        }

        return await query
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<Match?> GetByIdWithDetailsAsync(int id)
    {
        var query = await _dbSet
            .Where(m => m.Id == id)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Bets)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        // Select * From Matches

        return query;
    }
}
