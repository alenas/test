using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Battle {
	public class Battlefield {

		// this is not thread safe, - we could use ConcurentDictionary
		private readonly ConcurrentDictionary<string, Fighter> fighters = new ConcurrentDictionary<string, Fighter>();

		private readonly Action<string> Observer;

		public Battlefield(Action<string> observer) {
			Observer = observer;
		}

		/// <summary>
		/// Adds fighter to the battle
		/// </summary>
		public void AddFighter(Fighter fighter) {
			fighter.FighterActions += ObserveBattlefield;
			fighters.TryAdd(fighter.Name, fighter);
		}

		/// <summary>
		/// Adds list of fighters to the battle
		/// </summary>
		/// <param name="fighters"></param>
		public void AddFighters(IList<Fighter> fighters) {
			foreach (var fighter in fighters)
				AddFighter(fighter);
		}

		/// <summary>
		/// Starts battle
		/// </summary>
		public void StartFight() {
			if (fighters.Count < 1)
				throw new InvalidOperationException("There should be at least couple of fighters to start a battle!");

			// without setting a degree of parallelism, it would only use the same number as number of cores
			fighters.AsParallel().
				WithDegreeOfParallelism(fighters.Count).
				ForAll(f => f.Value.StartShootingAt(fighters));
			// print winner after fight finished
			ObserveBattlefield(fighters.First().Value, "is a winner!");
		}

		private void ObserveBattlefield(Fighter source, string action) {
			// remove dead fighters from the fight
			if (source.IsDead) fighters.TryRemove(source.Name, out _);
			Observer($"{source.Name}({source.Health}): {action}");
		}

	}
}
