using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace test {
	public class Program {
		/*
		readonly static string json = @"[
			{
			  'name': 'John',
			  'health': 10,
			  'damage': 1
			}
		]";
		/*/
		readonly static string json = @"[
			{
			  'name': 'John',
			  'health': 10,
			  'damage': 1
			},
			{
			  'name': 'Bill',
			  'health': 8,
			  'damage': 2
			},
			{
			  'name': 'Sam',
			  'health': 10,
			  'damage': 1
			},
			{
			  'name': 'Peter',
			  'health': 5,
			  'damage': 10
			},
			{
			  'name': 'Philip',
			  'health': 15,
			  'damage': 1
			}
		]";



		public static void Main(string[] args) {
			var fighters = Deserialize(json);
			// without setting a degree of parallelism, it would use only the same number as number of cores
			fighters.AsParallel().WithDegreeOfParallelism(fighters.Count).ForAll(f => f.Value.StartShootingAt(ref fighters));
			Console.WriteLine("Winner is: " + fighters.First().Value.Name);
			Console.ReadKey();
		}

		static ConcurrentDictionary<string, Fighter> Deserialize(string json) {
			var fighters = JsonConvert.DeserializeObject<List<Fighter>>(json);
			ConcurrentDictionary<string, Fighter> result = new ConcurrentDictionary<string, Fighter>(fighters.Count, fighters.Count);
			foreach (var f in fighters) {
				result.TryAdd(f.Name, f);
			}
			return result;
		}
	}
}
