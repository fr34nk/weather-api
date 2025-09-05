using System.Threading;
using System.Threading.Tasks;
using HappyCode.NetCoreBoilerplate.Core.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HappyCode.NetCoreBoilerplate.Core.Dtos;
using HappyCode.NetCoreBoilerplate.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace HappyCode.NetCoreBoilerplate.Core.Repositories
{
    public interface IWeatherRepository
    {

        Task<List<WeatherDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<WeatherDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<WeatherDetailsDto> GetByIdWithDetailsAsync(object id, CancellationToken cancellationToken);
        Task<WeatherDto> GetOldestAsync(CancellationToken cancellationToken);
        Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken);
        Task<Weather> InsertAsync(WeatherPostDto employeePostDto, CancellationToken cancellationToken);
        Task<WeatherDto> UpdateAsync(int id, WeatherPutDto employeePutDto, CancellationToken cancellationToken);
        Task<WeatherCollection> InsertMultiAsync(Weather[] weatherList, CancellationToken cancellationToken);
    }

    internal class WeatherRepository : RepositoryBase<Weather, WeatherContext>, IWeatherRepository
    {
        public WeatherRepository(WeatherContext dbContext, ILogger<IWeatherRepository> logger) : base(dbContext)
        { }

        public async Task<List<WeatherDto>> GetAllAsync (CancellationToken cancellationToken) {
            var weatherList = await DbContext.Weather
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return weatherList.Select(WeatherExtensions.MapToDto).ToList();
        }

        public async Task<WeatherDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var emp = await DbContext.Weather
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (emp == null)
            {
                return null;
            }

            return emp.MapToDto();
        }

        public async Task<WeatherDetailsDto> GetByIdWithDetailsAsync(object id, CancellationToken cancellationToken)
        {
            var emp = await DbContext.Weather
                .Include(x => x.Id)
                .SingleOrDefaultAsync(x => x.Id == (int)id, cancellationToken);

            if (emp == null) { return null; }

            return new WeatherDetailsDto
            {
                Id = emp.Id,
                Time = emp.Time,
                Humidity = emp.Humidity,
                Temperature = emp.Temperature,
                WindSpeed = emp.WindSpeed,
            };
        }

        public async Task<WeatherDto> GetOldestAsync(CancellationToken cancellationToken)
        {
            var emp = await DbContext.Weather
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (emp == null) { return null; }

            return emp.MapToDto();
        }

        public async Task<Weather> InsertAsync(WeatherPostDto weatherPostDto, CancellationToken cancellationToken)
        {
            var weather = new Weather
            {
                Time =  weatherPostDto.Time,
                Humidity = weatherPostDto.Humidity,
                Temperature = weatherPostDto.Temperature,
                WindSpeed = weatherPostDto.WindSpeed,
            };

            await DbContext.Weather.AddAsync(weather, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            return weather;
        }

        public async Task<WeatherCollection> InsertMultiAsync(Weather[] weatherList, CancellationToken cancellationToken)
        {
            try
            {
                WeatherCollection collection = new WeatherCollection();
                collection.AddRange(weatherList);

                await DbContext.Weather.AddRangeAsync(collection);
                await DbContext.SaveChangesAsync(cancellationToken);

                return collection;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("[InsertMultiAsync] Throw Exception {Ex}");
                throw;
            }
        }

        public async Task<WeatherDto> UpdateAsync(int id, WeatherPutDto weatherPutDto, CancellationToken cancellationToken)
        {
            var emp = await DbContext.Weather
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (emp is null)
            { return null; }

            emp.Id = weatherPutDto.Humidity;
            await DbContext.SaveChangesAsync(cancellationToken);
            return emp.MapToDto();
        }

        public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var emp = await DbContext.Weather
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (emp == null)
            { return false; }

            DbContext.Weather.Remove(emp);
            return await DbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
