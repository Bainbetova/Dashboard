using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dashboard
{
    public class AssetClass : INotifyPropertyChanged
    {
        private string category;
        public string Category
        {
            get { return category; }
            set
            {
                if(category == value)
                    return;

                category = value;
                RaisePropertyChanged(nameof(Category));
            }
        }

        private double cash;
        public double Cash
        {
            get { return cash; }
            set
            {
                cash = value;
                RaisePropertyChanged("Cash");
            }
        }

        private double card1;
        public double Card1
        {
            get { return card1; }
            set
            {
                card1 = value;
                RaisePropertyChanged("Card1");
            }
        }

        private double card2;

        public double Card2
        {
            get { return card2; }
            set
            {
                card2 = value;
                RaisePropertyChanged("Card2");
            }
        }

        public static List<AssetClass> ConstructTestData()
        {
            List<AssetClass> assetClasses = new List<AssetClass>();

            assetClasses.Add(new AssetClass() { Category = "Food", Cash = 10001, Card1 = 500, Card2 = 400 });
            assetClasses.Add(new AssetClass() { Category = "Auto", Cash = 0, Card1 = 400, Card2 = 170 });
            assetClasses.Add(new AssetClass() { Category = "Mobile", Cash = 1300, Card1 = 200, Card2 = 1 });
            assetClasses.Add(new AssetClass() { Category = "Cafe", Cash = 1600, Card1 = 1600, Card2 = 800 });
            assetClasses.Add(new AssetClass() { Category = "Education", Cash = 270, Card1 = 27, Card2 = 38 });
            assetClasses.Add(new AssetClass() { Category = "Flat", Cash = 5000, Card1 = 50, Card2 = 300 });

            return assetClasses;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}