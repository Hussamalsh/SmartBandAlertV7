﻿using Acr.UserDialogs;
using Plugin.BluetoothLE;
using Plugin.Geolocator;
using SmartBandAlertV7.Data;
using SmartBandAlertV7.Messages;
using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV7.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HemPage : ContentPage
    {
        IDisposable scan;


        //public IAppState AppState;

        // public string ScanText { get; private set; }
        public bool IsScanning { get; private set; }

        //public string Title { get; private set; }

        IDisposable subNoTIFY;


        public BLEAcrProfileManager bleACRProfileManager;
        public HemPage()
        {

            InitializeComponent();

            bleACRProfileManager = App.BLEAcrProfileManager;

            //theBTunits.IsPullToRefreshEnabled = true;

            stopDanger.BindingContext = new { w1 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc2 = Color.White };
            startDanger.BindingContext = new { w0 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc1 = Color.FromHex("#ededed") };

            //Battery 
            progBar.BindingContext = new { w4 = App.ScreenWidth * 160 / (App.ScreenDPI * 3), theprog = 1 };
            progBar.Scale = 1;
            batterystack.HorizontalOptions = LayoutOptions.CenterAndExpand;
            progBarText.BindingContext = new { theprogtext = "?%" };
            checkBattery.BindingContext = new { bgc3 = Color.White };

            startDanger.Clicked += async (s, e) =>
            {

                if (App.isConnectedBLE)
                {
                    //subNoTIFY.Dispose();
                    App.dangerModeOn = true;
                    

                    var message = new StartLongRunningTaskMessage();
                    MessagingCenter.Send(message, "StartLongRunningTaskMessage");

                    connectToBackend(true);

                }
                else
                    await DisplayAlert("Fel ", "Anslut till en enhet först.", "Ok");
                Task.Delay(500);
            };

            stopDanger.Clicked += (s, e) => {
                if (App.dangerModeOn)
                {
                    App.dangerModeOn = false;
                    connectToBackend(false);
                    App.ct.Cancel();
                    var message = new StopLongRunningTaskMessage();
                    MessagingCenter.Send(message, "StopLongRunningTaskMessage");
                }
            };

        }
        public async void connectToBackend(bool connect)
        {
            if (connect)
            {
                getLocationAsync(1);
                //activate danger mode in backend

                //App.VictimManager.setDM(victim, connect);
                //Task.Run(async () => uppdateLiveValueAsync());

            }
            else
            {
                getLocationAsync(11);
                //deactive danger mode in backend
                //App.VictimManager.setDM(getLocationAsync(), connect);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (bleACRProfileManager.bleprofile.BleAdapter.Status == AdapterStatus.PoweredOn)
            {

                if (bleACRProfileManager.bleprofile.Devices.Count == 0)
                {
                    bleACRProfileManager.intit();
                    IDevice dev = CrossBleAdapter.Current.GetConnectedDevices().FirstOrDefault();
                    if (dev != null)
                    {
                        bleACRProfileManager.bleprofile.Devices.Add(new ScanResultViewModel());
                        device = bleACRProfileManager.bleprofile.Devices.FirstOrDefault().TrySetCustom(dev);
                        this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;
                        efterConnection();
                    }
                }
                else
                {
                    this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;
                }

                //111
                bleACRProfileManager.bleprofile.BleAdapter.WhenScanningStatusChanged().Subscribe(on =>
                {
                    this.IsScanning = on;
                    this.ScanText.Text = on ? "Stoppa" : "Skanna";
                });
   
                
            }
            else
                await DisplayAlert("Bluetooth är avstängt", "Var vänlig starta bluetooth.", "Ok");
        }

        public async void Button_OnClickedScanToggle(object sender, EventArgs e)
        {
            if (bleACRProfileManager.bleprofile.BleAdapter.Status == AdapterStatus.PoweredOn)
            {
                if (this.IsScanning)
                {
                    this.scan?.Dispose();
                }
                else
                {
                    bleACRProfileManager.bleprofile.Devices.Clear();
                    this.ScanText.Text = "Stoppa";

                    this.scan = bleACRProfileManager.bleprofile.BleAdapter
                        .Scan()
                        .Subscribe(this.OnScanResult);
                }
            }
            else
                await DisplayAlert("Bluetooth är avstängt", "Var vänlig starta bluetooth.", "Ok");

        }

        void OnScanResult(IScanResult result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var dev = bleACRProfileManager.bleprofile.Devices.FirstOrDefault(x => x.Uuid.Equals(result.Device.Uuid));
                if (dev != null && !String.IsNullOrEmpty(dev.Name))
                {
                    dev.TrySet(result);
                }
                else
                {
                    dev = new ScanResultViewModel();
                    dev.TrySet(result);
                    if (!String.IsNullOrEmpty(dev.Name))
                        bleACRProfileManager.bleprofile.Devices.Add(dev);
                }
            });
            this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;

        }
        private bool dotwice = false;
        async void Button_OnClickedBatteriUppdat(Object obj, EventArgs e)
        {
            if (App.isConnectedBLE)
            {
                try
                {
                    byte[] toBytes = Encoding.UTF8.GetBytes("11");
                    Value = "newsendrequest";
                    bleACRProfileManager.bleprofile.CharacteristicWrite.WriteWithoutResponse(toBytes);
                    if (!dotwice)
                    {
                        bleACRProfileManager.bleprofile.CharacteristicWrite.WriteWithoutResponse(toBytes);
                        dotwice = true;
                    }
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.Alert(ex.ToString());
                }
            }
            else
                await DisplayAlert("Fel ", "Anslut till en enhet först.", "Ok");

        }

        public IDevice device;

        async void connectBClicked(object sender, EventArgs e)
        {
            bool answer;
            this.scan?.Dispose();

            var button = sender as Button;
            ScanResultViewModel item = button.BindingContext as ScanResultViewModel;
            device = item.Device;

            bleACRProfileManager.cleanEverything(device);
            try
            {
                // don't cleanup connection - force user to d/c
                if (this.device.Status == ConnectionStatus.Disconnected)
                {
                    answer = await DisplayAlert("Bluetooth", "Vill du ansluta till den här bluetooth enheten?", "Ja", "Nej");
                    if (answer == true)
                    {
                        using (var cancelSrc = new CancellationTokenSource())
                        {
                            using (UserDialogs.Instance.Loading("Ansluter", cancelSrc.Cancel, "Avbryt"))
                            {
                                await this.device.Connect().ToTask(cancelSrc.Token);
                            }

                            efterConnection();
                        }



                    }

                }
                else
                {
                    answer = await DisplayAlert("Bluetooth", "Vill du avbryta anslutningen?", "Ja", "Nej");
                    if (answer == true)
                    {
                        this.device.CancelConnection();
                        App.isConnectedBLE = false;
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.ToString());
            }
            //Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).UpdateD();
            this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;


        }

        public void efterConnection()
        {

            App.isConnectedBLE = true;

            this.device.WhenAnyCharacteristicDiscovered().Subscribe(characteristic =>
            {

                if (characteristic.Uuid.Equals(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e")))
                {
                    bleACRProfileManager.bleprofile.CharacteristicRead = characteristic;
                    // once you have your characteristic instance from the service discovery
                    // this will enable the subscriptions to notifications as well as actually hook to the event
                    subNoTIFY = bleACRProfileManager.bleprofile.CharacteristicRead.SubscribeToNotifications().Subscribe(result =>
                    { result.Characteristic.SubscribeToNotifications().Subscribe(x => this.SetReadValue(x, true)); });

                }
                if (characteristic.Uuid.Equals(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e")))
                {
                    bleACRProfileManager.bleprofile.CharacteristicWrite = characteristic;
                }

                //222


               /* bleACRProfileManager.bleprofile.device
                .WhenStatusChanged().TakeLast(1)
                .Subscribe(status =>
                           {
                               if (status == ConnectionStatus.Disconnected)
                               {
                                   // Navigate to new page
                                   if (App.dangerModeOn)
                                   {

                                       Device.StartTimer(TimeSpan.FromSeconds(300), () =>
                                       {
                                           if (!App.isConnectedBLE)
                                           {
                                               UserDialogs.Instance.Alert("DangerMode = On, Disconnected from the SBA device");
                                               Debug.WriteLine("DangerMode = Something Dangeres Happen");
                                               SendAlarm(true);
                                           }

                                           return false; // True = Repeat again, False = Stop the timer
                                       });
                                   }

                               }else if (status == ConnectionStatus.Connected || status == ConnectionStatus.Connecting)
                               {
                                   App.isConnectedBLE = true;
                               }

                           Debug.WriteLine(status.ToString());
                           });*/

            });
        }
        
        public string Value { get; set; }

        void SetReadValue(CharacteristicResult result, bool fromUtf8)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //this.IsValueAvailable = true;
                //this.LastValue = DateTime.Now;

                if (result.Data == null)
                    this.Value = "EMPTY";
                else
                {

                    string tempval = "emp";
                    if (String.IsNullOrEmpty(Value))
                    {
                        this.Value = System.Text.Encoding.UTF8.GetString(result.Data, 0, result.Data.Length - 1);
                    }
                    else
                    {
                        tempval = System.Text.Encoding.UTF8.GetString(result.Data, 0, result.Data.Length - 1);

                    }


                    if (!Value.Equals(tempval) && result.Data.Length < 5)
                    {
                        if (!tempval.Equals("emp"))
                            this.Value = tempval;
                        int nr = int.Parse(Value);
                        if (nr > 107)
                            await DisplayAlert("Laddar:", " Batteriet laddar.", "OK");
                        else
                        {
                            double resultlvl = (((double)nr / 107) * 100);
                            progBar.BindingContext = new { w4 = App.ScreenWidth * 160 / (App.ScreenDPI * 3), theprog = (resultlvl / 100) };
                            progBarText.BindingContext = new { theprogtext = resultlvl.ToString("#") + "%" };
                        }
                    }
                    else if (App.dangerModeOn && result.Data.Length > 5)
                    {
                        postAlarm();
                    }





                }
                //this.Value = fromUtf8 ? Encoding.UTF8.GetString(result.Data, 0, result.Data.Length) : BitConverter.ToString(result.Data);

            });
        }
        public bool postonce = false;
        public void postAlarm()
        {
            if (!postonce)
            {
                SendAlarm(true);

                Device.StartTimer(TimeSpan.FromSeconds(5), () =>
                {
                    // Do something
                    postonce = false;
                    return false; // True = Repeat again, False = Stop the timer
                });

                postonce = true;
            }
        }

        public async void SendAlarm(bool newornot)
        {
                getLocationAsync(2);
        }
        public Victim victim { set; get; } = new Victim();

        public async void getLocationAsync(int p)
        {
              await getLocationAsync1(p);
        }

        public async Task getLocationAsync1(int p)
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;


            var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);



            if (position == null)

            {

                //labelGPS.Text = "null gps :(";

                return;

            }

            /*labelGPS.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2} \n Altitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \n Heading: {6} \n Speed: {7}",

               position.Timestamp, position.Latitude, position.Longitude,

               position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);*/

            Geocoder geoCoder = new Geocoder();
            var fortMasonPosition = new Position(position.Latitude, position.Longitude);
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(fortMasonPosition);

            victim.FBID = App.FacebookId;
            victim.UserName = App.FacebookName;

            victim.StartDate = DateTime.Parse(position.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            victim.Latitude = "" + position.Latitude.ToString().Replace(",", ".");
            victim.Longitude = "" + position.Longitude.ToString().ToString().Replace(",", ".");
            victim.Adress = "" + possibleAddresses.FirstOrDefault();

            if (p == 1)
            {
                App.VictimManager.setDM(victim, true);
                Task.Run(async () => uppdateLiveValueAsync());
            }

            if (p == 2)
            {
                await App.VictimManager.SaveTaskAsync(victim, true);
            }

            if (p == 11)
            {
                App.VictimManager.setDM(victim, false);
            }

        }

        public async Task uppdateLiveValueAsync()
        {
            //App.VictimManager.setliveMode();


            App.ct = new CancellationTokenSource();
            while (!App.ct.IsCancellationRequested)
            {
                try
                {
                    //await Task.Delay(60000 - (int)(watch.ElapsedMilliseconds%1000), token);
                    App.ct.Token.ThrowIfCancellationRequested();
                    await Task.Delay(TimeSpan.FromSeconds(60), App.ct.Token).ContinueWith(async (arg) =>
                    {
                        if (!App.ct.Token.IsCancellationRequested)
                        {
                            App.ct.Token.ThrowIfCancellationRequested();
                            /*
							 * HERE YOU CAN DO YOUR ACTION
							 */
                            Device.BeginInvokeOnMainThread(() => App.VictimManager.setliveMode());
                        }
                    });
                    //Debug.WriteLine (DateTime.Now.ToLocalTime ().ToString () + " DELAY: " + delay);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("EX 1: " + ex.Message);
                }

            }

            //				try{
            //
            //					//Device.BeginInvokeOnMainThread(() => this.lblTimerText.Text = (DateTime.Now - startTime).ToString());
            //					//Device.BeginInvokeOnMainThread(() => this._labelOra.Text = watch.Elapsed.ToString());
            //					if (!App.CancellationToken.Token.IsCancellationRequested) {
            //						App.CancellationToken.Token.ThrowIfCancellationRequested();
            //						Device.BeginInvokeOnMainThread(()=> _label.Text = (++_counter).ToString());
            //						Debug.WriteLine ("TimerRunning " + _counter.ToString());// + watch.Elapsed.ToString ());
            //					}
            //				}
            //				catch (Exception ex){
            //					Debug.WriteLine("EX 2 " + ex.Message);
            //				}



        }



    }
}
