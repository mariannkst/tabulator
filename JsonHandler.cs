using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq;
using System.IO;

namespace JiraSyncTool
{
    public class JsonHandler
    {

        // Source of the simple version that was improved: https://stackoverflow.com/questions/11981282/convert-json-to-datatable
        public static DataTable Tabulate(string json)
        {
            var jsonLinq = JObject.Parse(json);

            // Find the first array using Linq
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children<JObject>())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Include JValue types
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                    // Check if it is an array
                    else if (column.Value is JArray)
                    {
                        string sA = column.Value.ToString();
                        JArray arr = JArray.Parse(sA);
                        JArray arrValues = new JArray();

                        for (int i = 0; i < arr.Count(); i++)
                        {
                            if (arr[i] is JValue)
                            {
                                arrValues.Add(arr[i]);
                            }
                        }
                        cleanRow.Add(column.Name, arrValues.ToString());

                    }
                    // Check if it is a JObject
                    else if (column.Value is JObject)
                    {
                        string sJ2 = column.Value.ToString();
                        JObject jO2 = JObject.Parse(sJ2);

                        // Dig down in the Object and repeat the checks for extracting more data
                        foreach (JProperty jp2 in jO2.Properties())
                        {
                            if (jp2.Value is JValue)
                            {
                                cleanRow.Add(column.Name + "_" + jp2.Name, jp2.Value);
                            }
                            else if (jp2.Value is JArray)
                            {
                                string sA3 = jp2.Value.ToString();
                                JArray arr3 = JArray.Parse(sA3);
                                JArray arrValues3 = new JArray();

                                for (int i = 0; i < arr3.Count(); i++)
                                {
                                    if (arr3[i] is JValue)
                                    {
                                        arrValues3.Add(arr3[i]);
                                    }
                                }
                                cleanRow.Add(column.Name + "_" + jp2.Name, arrValues3.ToString());
                            }
                            else if (jp2.Value is JObject)
                            {
                                string sJ3 = jp2.Value.ToString();
                                JObject jO3 = JObject.Parse(sJ3);

                                // Dig down in the Object and repeat the checks for extracting more data
                                foreach (JProperty jp3 in jO3.Properties())
                                {
                                    if (jp3.Value is JValue)
                                    {
                                        cleanRow.Add(column.Name + "_" + jp2.Name + "_" + jp3.Name, jp3.Value);
                                    }
                                    else if (jp3.Value is JArray)
                                    {
                                        string sA4 = jp3.Value.ToString();
                                        JArray arr4 = JArray.Parse(sA4);
                                        JArray arrValues4 = new JArray();

                                        for (int i = 0; i < arr4.Count(); i++)
                                        {
                                            if (arr4[i] is JValue)
                                            {
                                                arrValues4.Add(arr4[i]);
                                            }
                                        }
                                        cleanRow.Add(column.Name + "_" + jp2.Name + "_" + jp3.Name, arrValues4.ToString());
                                    }
                                    else if (jp3.Value is JObject)
                                    {

                                        string sJ4 = jp3.Value.ToString();
                                        JObject jO4 = JObject.Parse(sJ4);

                                        // Dig down in the Object and repeat the checks for extracting more data
                                        foreach (JProperty jp4 in jO4.Properties())
                                        {
                                            if (jp4.Value is JValue)
                                            {
                                                cleanRow.Add(column.Name + "_" + jp2.Name + "_" + jp3.Name + "_" + jp4.Name, jp4.Value);
                                            }
                                            else if (jp4.Value is JArray)
                                            {
                                                string sA5 = jp4.Value.ToString();
                                                JArray arr5 = JArray.Parse(sA5);
                                                JArray arrValues5 = new JArray();

                                                for (int i = 0; i < arr5.Count(); i++)
                                                {
                                                    if (arr5[i] is JValue)
                                                    {
                                                        arrValues5.Add(arr5[i]);
                                                    }
                                                }
                                                cleanRow.Add(column.Name + "_" + jp2.Name + "_" + jp3.Name + "_" + jp4.Name, arrValues5.ToString());
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                trgArray.Add(cleanRow);
            }

            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }

        

    }
}
