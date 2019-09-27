using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Battle {

	public class Fighter {

		public string Name { get; }

		public int Health { get; private set; }

		public int Damage { get; }

		public int ReloadTime { get; }

		public bool IsDead { get { return Health <= 0; } }

		private ConcurrentDictionary<string, Fighter> fighters;

		private static Random random = new Random((new Random()).Next(int.MaxValue));

		public event Action<Fighter, string> FighterActions;

		public Fighter(string name, int health, int damage) {
			Name = name;
			Health = health;
			Damage = damage;
			ReloadTime = 500;
		}

		public void StartShootingAt(ConcurrentDictionary<string, Fighter> fighters) {
			FighterActions(this, "starts shooting");
			this.fighters = fighters;
			while (ShootRandom()) {
				ReloadGun();
			}
			// remove cross reference so GC could collect
			this.fighters = null;
		}

		private void ReloadGun() {
			Thread.Sleep(ReloadTime);
		}

		/// <summary>
		/// inflicts damage and returns if fighter is dead
		/// </summary>
		/// <param name="damage">Damage to inflict</param>
		/// <returns>true if dead</returns>
		private bool InflictDamage(int damage) {
			Health -= damage;
			if (IsDead) FighterActions(this, "is dead");
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
				FighterActions(this, $"shoots at {fighter.Name} {fighter.Health}-{Damage}");
				fighter.InflictDamage(Damage);
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
			return fighters.Values.ElementAt(i);
		}

	}
}
