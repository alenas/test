using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace test {

	public class Fighter {

		public string Name { get; }

		public int Health { get; private set; }

		public int Damage { get; }

		public bool IsDead { get { return Health <= 0; } }

		private ConcurrentDictionary<string, Fighter> fighters;

		public const int SleepIntervalMs = 500;

		private static Random random = new Random((new Random()).Next(int.MaxValue));

		public Fighter(string name, int health, int damage) {
			Name = name;
			Health = health;
			Damage = damage;
		}

		public void StartShootingAt(ref ConcurrentDictionary<string, Fighter> fighters) {
			Log("starts shooting");
			this.fighters = fighters;
			while (ShootRandom()) {
				Thread.Sleep(SleepIntervalMs);
			}
			// remove cross reference so GC could collect
			this.fighters = null;
		}

		/// <summary>
		/// inflicts damage and returns if fighter is dead
		/// </summary>
		/// <param name="damage">Damage to inflict</param>
		/// <returns>true if dead</returns>
		private bool InflictDamage(int damage) {
			Health -= damage;
			if (IsDead) Log("is dead");
			return IsDead;
		}

		/// <summary>
		/// Shoots random player
		/// </summary>
		/// <returns>true if we can shoot again</returns>
		bool ShootRandom() {
			if (IsDead) {
				// fighter is already dead
				return false;
			}
			var fighter = GetSuitable();
			if (fighter != null) {
				Log($"shoots at {fighter.Name} {fighter.Health}-{Damage}");
				if (fighter.InflictDamage(Damage)) {
					fighters.TryRemove(fighter.Name, out _);
				}
				return true;
			} else {
				return false;
			}

		}

		Fighter GetSuitable() {
			do {
				var fighter = GetRandom();
				if (fighter != this && !fighter.IsDead) {
					return fighter;
				}
			} while (fighters.Count > 1);

			return null;
		}

		Fighter GetRandom() {
			// when there are two fighters left, we could select the OTHER fighter, instead of random
			var i = random.Next(fighters.Count);
			return fighters.ElementAt(i).Value;
		}

		void Log(string action) {
			Console.WriteLine($"{Name}({Health}): {action}");
		}

	}
}
