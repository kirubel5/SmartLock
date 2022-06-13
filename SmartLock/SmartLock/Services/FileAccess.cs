using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace SmartLock.Services
{
    public static class FileAccess
    {      
        //This method Randomly generates ID
        public static string GetNewId()
        {
            List<string> ids = GetIdList();

            Random random = new Random();
            int newId = new Random().Next(1, 127);

            while(ids.Contains(newId.ToString()))
            {
                newId = random.Next(1, 127);
            }

            return newId.ToString();
        }

        //This method deletes Id from the phone when a registered user is deleted
        //making the ID available for use
        public static void DeleteIdFromFile(string id)
        {         
            try
            {
                List<string> ids = GetIdList();
                _ = ids.Remove(id);

                string file = "";

                foreach (var item in ids)
                {
                    file += (ids + "\n");
                }

                string filePath = Path.Combine(FileSystem.AppDataDirectory, "id");
                File.WriteAllText(filePath, file);
            }
            catch
            {
                return;
            }
        }

        //This method gets Ids already used so that they wont be used again
        private static List<string> GetIdList()
        {
            List<string> ids = new List<string>();

            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "id");

                if (File.Exists(filePath))
                {
                    string line = "";

                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        do
                        {
                            line = reader.ReadLine();

                            if (line != null)
                                ids.Add(line);
                        } while (line != null);
                    }
                }

                return ids;
            }
            catch
            {
                throw;
            }
        }

        //This method saves an id number when a user is registered, making it unavailable for used
        public static void AddIdToFile(string id)
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "id");
                string file = id + "\n";
                File.AppendAllText(filePath, file);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
