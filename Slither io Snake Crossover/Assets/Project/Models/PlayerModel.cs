using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace ClasePlayfab
{
    [Serializable]
    public class PlayerModel
    {
        public string City;

        public float CurrentHealth;
        public float Strength;

        private void SetCity(string city)
        {
            City = city;
        }

        private void SetCurrentHealth(string health)
        {
            CurrentHealth = float.Parse(health);
        }

        private void SetCurrentStrength(string str)
        {
            Strength = float.Parse(str);
        }

        public static PlayerModel LoadPlayerData(Dictionary<string, UserDataRecord> data)
        {
            PlayerModel model = new PlayerModel();

            model.SetCity(data["City"].Value);
            model.SetCurrentHealth(data["CurrentHealth"].Value);
            model.SetCurrentStrength(data["Strength"].Value);

            return model;
        }
    }
}