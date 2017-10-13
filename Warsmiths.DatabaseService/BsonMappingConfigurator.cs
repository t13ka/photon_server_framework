using MongoDB.Bson.Serialization;

using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Tasks;

namespace Warsmiths.DatabaseService
{
    /// <summary>
    /// </summary>
    public static class BsonMappingConfigurator
    {
        /// <summary>
        /// </summary>
        public static void Configure()
        {
            var domainConfig = new DomainConfiguration(true);

            BsonClassMap.RegisterClassMap<IEntity>(
                cm =>
                    {
                        cm.AutoMap();
                        cm.SetIsRootClass(true);
                        cm.MapIdField("_id");
                    });

            // mapping for all objects
            foreach (var baseEquipment in domainConfig.Objects)
            {
                BsonClassMap.LookupClassMap(baseEquipment.GetType());
            }
            
            BsonClassMap.LookupClassMap(typeof(BaseReciept));
            BsonClassMap.LookupClassMap(typeof(BaseQuest));
            BsonClassMap.LookupClassMap(typeof(Task));
            BsonClassMap.LookupClassMap(typeof(TaskCraftQuestReciept));

            foreach (var b in domainConfig.Items)
            {
                BsonClassMap.LookupClassMap(b.GetType());
            }
        }
    }
}