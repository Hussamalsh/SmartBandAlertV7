using Backendt1.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backendt1
{
    public class LocationPersistence
    {
        public string connectionString = "Database=sbadb;Data Source=eu-cdbr-azure-west-a.cloudapp.net;User Id=b8788f67037f24;Password=89de9c53";

        public String saveUserLocation(Location locationToSave)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                String sqlString = "INSERT INTO location (fbid, name, latitude, longitude)"
                    + "VALUES(" + locationToSave.FBID + ",'" + locationToSave.UserName + "','" 
                    + locationToSave.Latitude.ToString().Replace(",", ".") + "','" + locationToSave.Longitude.ToString().Replace(",", ".") + "');";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                cmd.ExecuteNonQuery();

                //string fbid = cmd.LastInsertedId;
                return locationToSave.FBID;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }

        public string editLocation(Location userlocObj)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();

                String sqlString = "UPDATE location SET latitude="+ userlocObj.Latitude.ToString().Replace(",", ".") +
                                                     ",longitude="+userlocObj.Longitude.ToString().Replace(",", ".") +
                                                     "WHERE fbid="+userlocObj.FBID;

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                cmd.ExecuteNonQuery();

                //string fbid = cmd.LastInsertedId;
                return userlocObj.FBID;

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }


        public ArrayList nearbyLocations(string latitude, string longitude)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();

                ArrayList personArrayL = new ArrayList();

                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                //String sqlString = "SELECT * FROM location WHERE FBID =" + ID + " ORDER BY startDate DESC LIMIT 1";
                String sqlstring1 = "select * from(SELECT fbid,name,latitude,longitude,round((((acos(sin(("+latitude+
                    "*pi()/180)) *sin((`latitude`*pi()/180))+cos(("+latitude+
                    "*pi()/180)) * cos((`latitude`*pi()/180)) *cos((("+longitude+
                    "- `longitude`)*pi()/180))))*180/pi())*60*1.1515*1.609344),1) as distance FROM location) as temp where distance <50";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlstring1, conn);

                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    Location u = new Location();

                    u.FBID = mySQLReader.GetString(0);
                    u.UserName = mySQLReader.GetString(1);
                    u.Latitude = mySQLReader.GetDecimal(2).ToString();
                    u.Longitude = mySQLReader.GetDecimal(3).ToString();
                    u.Distance = mySQLReader.GetDecimal(4).ToString()+" Km";
                    personArrayL.Add(u);
                }


                return personArrayL;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }



        public ArrayList getallLocations()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();

            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();

                ArrayList personArrayL = new ArrayList();

                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                //String sqlString = "SELECT * FROM location WHERE FBID =" + ID + " ORDER BY startDate DESC LIMIT 1";
                String sqlstring1 = "select * from location";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlstring1, conn);

                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    Location u = new Location();

                    u.FBID = mySQLReader.GetString(0);
                    u.UserName = mySQLReader.GetString(1);
                    u.Latitude = mySQLReader.GetDecimal(2).ToString();
                    u.Longitude = mySQLReader.GetDecimal(3).ToString();
                    u.Distance = mySQLReader.GetDecimal(4).ToString() + " Km";
                    personArrayL.Add(u);
                }


                return personArrayL;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }



        }
}