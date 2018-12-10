using Microsoft.SharePoint;
using SP2016.Repository.Batch;
using SP2016.Repository.Entities;
using SP2016.Repository.Mapping.SharePoint;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SP2016.Repository.Utils
{
    public static class BatchUtil
    {
        public static void ProcessBatch<TEntity>(TEntity[] entities, SPWeb web, string listName, SPBatchMapper<TEntity> spBatchMapper, string commandFormat, int blocksize) where TEntity : BaseEntity
        {
            if (blocksize < 1)
                throw new ArgumentOutOfRangeException("blocksize", "Размер блока должен быть натуральным числом");

            if (entities == null || entities.Length < 1)
                return;

            SPList list = web.Lists[listName];

            using (new AllowUnsafeUpdates(web))
            {
                StringBuilder builder = null;

                for (int i = 0; i < entities.Length; i++)
                {
                    if (i % blocksize == 0)
                    {
                        if (builder != null)
                        {
                            FinishBatch(entities, web, builder);
                        }
                        builder = new StringBuilder();
                        builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch OnError=\"Continue\">");
                    }

                    StringBuilder batchCommand = new StringBuilder();
                    spBatchMapper.Map(web, list, batchCommand, entities[i]);
                    string command = string.Format(commandFormat, i, list.ID, entities[i].ID, batchCommand.ToString());

                    builder.Append(command);
                }

                FinishBatch(entities, web, builder);
            }
        }

        public static void FinishBatch<TEntity>(TEntity[] entities, SPWeb web, StringBuilder builder) where TEntity : IEntity
        {
            string response = FinishBatch(web, builder);
            if (string.IsNullOrEmpty(response)) return;
            Results results = DeserializeResults(response);
            if (results != null)
            {
                foreach (ResultEntry result in results.Result)
                {
                    if (result.MethodID.HasValue)
                    {
                        var entity = entities[result.MethodID.Value];

                        if (result.Code != 0)
                        {
                            string message = $"Тип сущности: {entity.GetType()}. Текст ошибки: {result.ErrorText}";
                            throw new InvalidOperationException($"Произошла ошибка при выполнении команды {message}");
                        }

                        if (result.ItemID.HasValue)
                            entity.ID = result.ItemID.Value;
                    }
                    else
                    {
                        if (result.Code != 0)
                            throw new InvalidOperationException($"Произошла ошибка при выполнении команды {result.ErrorText}");
                    }
                }
            }
        }

        public static Results DeserializeResults(string xml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Results));
            using (TextReader reader = new StringReader(xml))
            {
                return xmlSerializer.Deserialize(reader) as Results;
            }
        }

        public static void ProcessBatch(int count, Func<int, string> methodExtractor, SPWeb web, int blocksize, Action<int> batchFinishedFunc)
        {
            if (blocksize < 1) throw new ArgumentOutOfRangeException("blocksize", "Размер блока должен быть натуральным числом");
            if (count < 1) return;

            using (new AllowUnsafeUpdates(web))
            {
                StringBuilder builder = null;
                for (int i = 0; i < count; i++)
                {
                    if (i % blocksize == 0)
                    {
                        if (builder != null)
                        {
                            FinishBatch(web, builder);
                            batchFinishedFunc?.Invoke(i);
                        }
                        builder = new StringBuilder();
                        builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch OnError=\"Continue\">");
                    }

                    builder.Append(methodExtractor(i));
                }

                FinishBatch(web, builder);
                batchFinishedFunc?.Invoke(count);
            }
        }

        public static string FinishBatch(SPWeb web, StringBuilder builder)
        {
            if (builder != null && web != null)
            {
                builder.Append("</Batch>");
                string request = builder.ToString();
                string response = web.ProcessBatchData(request);
                web.Update();
                return response;
            }
            return null;
        }
    }
}
