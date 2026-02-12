using MongoDB.Bson;
using MongoDB.Driver;
using Grid_MongoDB.Models;

namespace Grid_MongoDB.Services
{
    public class MongoDbService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Project> _collection;
        private const string ProjectIdPrefix = "PROJ";
        private const int ProjectIdStartNumber = 1;

        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase("ProjectManagementDB");
            _collection = _database.GetCollection<Project>("Projects");
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            try
            {
                return await _collection.Find(new BsonDocument()).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching projects: {ex.Message}");
            }
        }

        public async Task<Project> InsertProjectAsync(Project project)
        {
            if (string.IsNullOrEmpty(project.ProjectId))
            {
                project.ProjectId = await GenerateProjectIdAsync();
            }
            
            if (project.StartDate == null)
            {
                project.StartDate = DateTime.Now;
            }
            
            await _collection.InsertOneAsync(project);
            return project;
        }

        private async Task<string> GenerateProjectIdAsync()
        {
            var existingProjects = await GetProjectsAsync();
            int maxNumber = existingProjects
                .Where(project => !string.IsNullOrEmpty(project.ProjectId) && project.ProjectId.StartsWith(ProjectIdPrefix))
                .Select(project =>
                {
                    string numberPart = project.ProjectId.Substring(ProjectIdPrefix.Length);
                    if (int.TryParse(numberPart, out int number))
                        return number;
                    return 0;
                })
                .DefaultIfEmpty(ProjectIdStartNumber - 1)
                .Max();

            int nextNumber = maxNumber + 1;
            string newProjectId = $"{ProjectIdPrefix}{nextNumber:D3}";
            return newProjectId;
        }

        public async Task<bool> UpdateProjectAsync(string projectId, Project project)
        {
            var filter = Builders<Project>.Filter.Eq(p => p.ProjectId, projectId);
            var result = await _collection.ReplaceOneAsync(filter, project);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProjectAsync(string? projectId)
        {
            var filter = Builders<Project>.Filter.Eq(p => p.ProjectId, projectId);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}
