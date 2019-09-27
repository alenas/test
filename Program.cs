using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Battle {

	class Program {

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
			  'damage': 10
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


		static void Main(string[] args) {
			// log battle actions to console
			var battleField = new Battlefield(action => Console.WriteLine(action));
			// read fighters from JSON
			var fighters = Deserialize(json);

			battleField.AddFighters(fighters);
			battleField.StartFight();
		}

		static IList<Fighter> Deserialize(string json) {
			return JsonConvert.DeserializeObject<IList<Fighter>>(json);
		}
	}
}
