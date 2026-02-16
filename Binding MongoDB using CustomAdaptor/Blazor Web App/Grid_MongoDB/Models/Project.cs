using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Grid_MongoDB.Models
{
    public class Project
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("projectId")]
        public string ProjectId { get; set; } = string.Empty;

        [BsonElement("projectName")]
        public string ProjectName { get; set; } = string.Empty;

        [BsonElement("client")]
        public string Client { get; set; } = string.Empty;

        [BsonElement("projectManager")]
        public string? ProjectManager { get; set; }

        [BsonElement("department")]
        public string? Department { get; set; }

        [BsonElement("startDate")]
        public DateTime? StartDate { get; set; }

        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }

        [BsonElement("budget")]
        public decimal Budget { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Planning"; // Planning, In Progress, Completed, On Hold, Cancelled

        [BsonElement("priority")]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        [BsonElement("progress")]
        public int Progress { get; set; }

        [BsonElement("teamSize")]
        public int TeamSize { get; set; }

        [BsonElement("category")]
        public string? Category { get; set; }
    }
}
