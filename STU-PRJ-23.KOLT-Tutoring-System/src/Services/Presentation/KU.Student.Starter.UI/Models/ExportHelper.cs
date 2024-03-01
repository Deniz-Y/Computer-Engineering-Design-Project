using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web;
using static Npgsql.PostgresTypes.PostgresCompositeType;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace KU.Student.Starter.UI.Models
{
    public class ExportHelper
    {
        public MemoryStream Export<T>(List<T> genericList)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter objstreamwriter = new StreamWriter(stream);
                string clientHeader = String.Empty;
                var listOfFieldNames = genericList.First().GetType().GetProperties().Select(f => f.Name).ToList();
                bool skipFlag = false;

                if (!(listOfFieldNames[0].ToLower() == "id"))
                {
                    clientHeader += String.Format("{0},", listOfFieldNames[0]);
                    skipFlag = true;
                }

                for (int i = 1; i < listOfFieldNames.Count - 1; i++)
                {
                    clientHeader += String.Format("{0},", listOfFieldNames[i]);
                }
                clientHeader += String.Format("{0}" + Environment.NewLine, listOfFieldNames.Last());

                objstreamwriter.Write(clientHeader);


                String line = String.Empty;

                foreach (var item in genericList)
                {
                    if(skipFlag)
                    {
                        line = String.Format($"{item?.GetType()?.GetProperty(listOfFieldNames[0])?.GetValue(item, null)},");
                        objstreamwriter.Write(line);
                    }

                    for (int i = 1; i < listOfFieldNames.Count - 1; i++)
                    {

                        line = String.Format($"{item?.GetType()?.GetProperty(listOfFieldNames[i])?.GetValue(item, null)},");
                        objstreamwriter.Write(line);

                    }
                    line = String.Format($"{item?.GetType()?.GetProperty(listOfFieldNames.Last())?.GetValue(item, null)}{Environment.NewLine}");
                    objstreamwriter.Write(line);
                }
                
                objstreamwriter.Flush();
                objstreamwriter.Close();
                return stream;
            }
        }
    }
}