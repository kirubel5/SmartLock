using SmartLock.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartLock.Services
{
    public static class PeopleCache
    {
        //This property defines list of PersonModel
        public static List<PersonModel> People { get; set; } = new List<PersonModel>();

        //This method caches the list received from the arduino
        public static void AddListToCache(List<PersonModel> models)
        {
            People.Clear();//clear the cache before adding new list
            People.AddRange(models);
        }

        //This method adds a Person to the cach during registration
        public static void AddToCache(PersonModel model)
        {
            People.Add(model);
        }

        //This method deletes a Person from cach during delete operation
        public static void DeleteFromCache(string id)
        {
            List<PersonModel> people = new List<PersonModel>();

            foreach (var item in People)
            {
                if (item.Id != id)
                {
                    people.Add(item);
                }
            }

            People.Clear();
            if (people == null || people.Count == 0)
                return;

            People.AddRange(people);
        }

        //This method clears all people from the cach
        public static void ClearPeopleCache()
        {
            People.Clear();
        }

        //This method takes all the people inside the cash and converts them to a single
        //long string to be sent to the arduino
        public static string GetListInSingleStringForm()
        {
            string data = "";

            foreach (var item in People)
            {
                data += (item.Id + " " + item.Name + "\r\n");
            }

            return data;
        }

    }
}
