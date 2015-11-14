using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrewClass;
using UpgradeClass;
using CargoClass;

namespace CrewClass
{
    public class Crew
    {
        
    }
}
namespace UpgradeClass
{
    public class Upgrade
    {

    }
}
namespace CargoClass
{
    public class Cargo
    {

    }
}

namespace ShipClass
{
    public class BaseShip
    {
        public int mHullHealth
        {
            get { return mHullHealth; }
            set { mHullHealth = value; }
        }
        public int mSailHealth
        {
            get { return mSailHealth; }
            set { mSailHealth = value; }
        }
        public List< Crew > mShipCrew
        {
            get { return mShipCrew; }
            set { mShipCrew = value; }
        }
        public double mCrewMorale
        {
            get { return mCrewMorale; }
            set { mCrewMorale = value; }
        }
        public int mCargoSpace
        {
            get { return mCargoSpace; }
            set { mCargoSpace = value; }
        }
        public List< Upgrade > mShipUpgrades
        {
            get { return mShipUpgrades; }
            set { mShipUpgrades = value; }
        }
        public string mShipName
        {
            get { return mShipName; }
            set { mShipName = value; }
        }
        public List< Cargo > mShipCargo;

        bool CheckCargoSpace(  )
        {
            bool canHoldMore = false;

            if( mShipCargo.Count() < mCargoSpace )
                canHoldMore = true;
            
            return canHoldMore;
        }
        bool CheckCargo( Cargo toCheck )
        {
            return mShipCargo.Contains( toCheck );
        }
        bool AddCargo( Cargo toStore )
        {
            bool cargoWasStored = false;

            if( CheckCargoSpace( ) )
            {
                this.mShipCargo.Add( toStore );
                this.mShipCargo.Sort( );
                cargoWasStored = true;
            }

            return cargoWasStored;
        }
        bool RemoveCargo( Cargo toRemove )
        {
            bool cargoWasRemoved = false;

            if( mShipCargo.Contains( toRemove ) )
            {
                mShipCargo.Remove( toRemove );
                mShipCargo.Sort(  );
                cargoWasRemoved = true;
            }

            return cargoWasRemoved;
        }
        bool TradeCargo( Cargo toTrade, BaseShip from )
        {
            bool tradeSucceeded = false;

            if( this.CheckCargoSpace(  ) )
            {
                if( from.CheckCargo( toTrade ) )
                {
                    this.AddCargo( toTrade );
                    this.mShipCargo.Sort(  );
                    from.RemoveCargo( toTrade );
                    from.mShipCargo.Sort(  );
                    tradeSucceeded = true;
                }
            }

            return tradeSucceeded;
        }

        void DestroyShip(  )
        {
            mHullHealth = 0;
            mSailHealth = 0;
        }
    }
}
