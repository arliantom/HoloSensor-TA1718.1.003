using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Urho;
using Urho.Actions;
using Urho.SharpReality;
using Urho.Shapes;
using Urho.Resources;
using RabbitMQ.Client;
using System.Diagnostics;
using Urho.Gui;
using Windows.Media.MediaProperties;
using System.IO;
using Windows.Storage.Streams;
using Urho.Urho2D;
using Windows.Media.Capture;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Media.Effects;
using Windows.Foundation.Collections;
using Newtonsoft.Json;


namespace TA1718._1._003
{
    internal class Program
    {
        [MTAThread]
        static void Main()
        {
            var appViewSource = new UrhoAppViewSource<HelloWorldApplication>(new ApplicationOptions("Data"));
            appViewSource.UrhoAppViewCreated += OnViewCreated;
            CoreApplication.Run(appViewSource);
        }

        static void OnViewCreated(UrhoAppView view)
        {
            view.WindowIsSet += View_WindowIsSet;
        }

        static void View_WindowIsSet(Windows.UI.Core.CoreWindow coreWindow)
        {
            // you can subscribe to CoreWindow events here
        }
    }


    public class HelloWorldApplication : StereoApplication
    {
        // Node Untuk Pilihan 3 Dimensi  
        Node JudulHoloSensor, Kotak3D, Kotak2D, Teks3D, Teks2D, Kotak4Sensor, Teks4Sensor, KotakJarak, TeksJarak;

        // Node Untuk Pilihan Tiap Node
        Node suhuNode, suhu2Node, textTemp, textTempRek, frontsuhu, backsuhu, upsuhu, rightsuhu, downsuhu, leftsuhu, plussuhu, bplussuhu, minsuhu, bminsuhu, rotate_pos_suhu, rotate_neg_suhu;
        Node lembapNode, textHum, textHumRek, fronthum, backhum, uphum, righthum, downhum, lefthum, plushum, bplushum, minhum, bminhum, rotate_pos_hum, rotate_neg_hum;
        Node cahayaNode, textIntensRek, cahaya2Node, cahaya3Node, textIntens, frontchy, backchy, upchy, rightchy, downchy, leftchy, pluschy, bpluschy, minchy, bminchy, rotate_pos_chy, rotate_neg_chy;
        Node bisingNode, textNoise, textNoiseRek, frontnoise, backnoise, upnoise, rightnoise, downnoise, leftnoise, plusnoise, bplusnoise, minnoise, bminnoise, rotate_pos_noise, rotate_neg_noise;

        // Node Untuk Lampu, Kipas, dan Jarak Sensor
        Node textjudullampu, lampu1plus, lampu1min, lampu2plus, lampu2min, lampu3plus, lampu3min, lampu4plus, lampu4min, nilailampu1, nilailampu2, nilailampu3, nilailampu4, textlampu1, textlampu2, textlampu3, textlampu4, frontlampu, backlampu, uplampu, rightlampu, downlampu, leftlampu, pluslampu, bpluslampu, minlampu, bminlampu, rotate_pos_lampu, rotate_neg_lampu;
        Node textjudulkipas, kipas1plus, kipas1min, kipas2plus, kipas2min, nilaikipas1, nilaikipas2, textkipas1, textkipas2, frontkipas, backkipas, upkipas, rightkipas, downkipas, leftkipas, pluskipas, bpluskipas, minkipas, bminkipas, rotate_pos_kipas, rotate_neg_kipas;
        Node textjuduljarak, jarak1plus, jarak1min, jarak2plus, jarak2min, jarak3plus, jarak3min, nilaijarak1, nilaijarak2, nilaijarak3, textjarak1, textjarak2, textjarak3, frontjarak, backjarak, upjarak, rightjarak, downjarak, leftjarak, plusjarak, bplusjarak, minjarak, bminjarak, rotate_pos_jarak, rotate_neg_jarak;

        // Node untuk Mode Kelas
        Node PilihanModeKelas, TeksDigitKelas, Pilihan1, Pilihan2, Pilihan3, Pilihan4, /*digitkelas1plus, digitkelas1min, digitkelas2plus, digitkelas2min, digitkelas3plus, digitkelas3min, digitkelas4plus, digitkelas4min,*/ nilaidigitkelas1, nilaidigitkelas2, nilaidigitkelas3, nilaidigitkelas4, frontkelas, backkelas, upkelas, rightkelas, downkelas, leftkelas, pluskelas, bpluskelas, minkelas, bminkelas;

        // Sprite 2D untuk Penunjuk bagian tengah
        Sprite crosshair;

        // Struct untuk Tipe Data posisi 3 Dimensi dan Skala masing-masing Node
        public struct modifikasi
        {
            public float x, y, z, skala, r;

            public modifikasi(float p1, float p2, float p3, float p4, int p5, int p6, int p7)
            {
                x = p1;
                y = p2;
                z = p3;
                skala = p4;
                r = p5;
            }
        }

        modifikasi kotak, temp, hum, intens, noise, lampu, kipas, kelas, slider1plus, slider1min, slider2plus, slider2min, slider3plus, slider3min, slider4plus, slider4min, slider5plus, slider5min, slider6plus, slider6min, jarak;

        public HelloWorldApplication(ApplicationOptions opts) : base(opts) { }

        private UIElement uiRoot;

        // Variabel Menyimpan Nilai Integer masing-masing Aktuator dan Mode Kelas
        public int nlampu1, nlampu2, nlampu3, nlampu4, nkipas1, nkipas2, nmode, njarak1, njarak2, njarak3;

        // Variabel Boolean Sebagai Flag
        public bool KondisiRuang_2D;
        public bool KondisiRuang_3D;
        public bool KondisiRuang_4Sensor;
        public bool VoiceCommand_2D;
        public bool Gesture_2D;
        public bool Face_2D;
        public bool VoiceCommand_Accepted;
        public bool JarakSensor;

        // Variabel Temporary
        string VoiceCommand_PerintahDiterima;

        // Memanggil tiap UI 2 Dimensi
        UITwoD ui;
        UITwoD_4Sensor ui_4Sensor;
        UITwoD_Voice ui_Voice;
        UITwoD_Face ui_Face;
        UITwoD_Gesture ui_Gesture;

        RMQ rabbitmq;

        // Data JSON masing-masing Kelompok
        sensormessage dataJson;
        voicemessage dataJson2;
        facemessage dataJson3;
        gesturemessage dataJson4;

        protected override async void Start()
        {
            //ResourceCache.AutoReloadResources = true;
            // Create a basic scene, see StereoApplication
            base.Start();

            uiRoot = UI.Root;

            ShowCrosshair();

            // UI.Root.AddChild(crosshair);

            // Enable input
            EnableGestureManipulation = true;
            EnableGestureTapped = true;
            EnableGestureHold = true;

            rabbitmq = new RMQ();

            // Konfigurasi Voice Command agar bisa diterima
            VoiceCommand_PerintahDiterima = "ian\n";

            // Pembuatan Channel Masing-masing Data
            rabbitmq.createChannel("holosensor", "amq.topic", "holosensor.data");
            rabbitmq.createChannel_Voice("Voice", "amq.topic", "voice.aktuator");
            rabbitmq.createChannel_Face("Face", "amq.topic", "face.aktuator");
            rabbitmq.createChannel_Gesture("Gesture", "amq.topic", "gesture.aktuator");
            rabbitmq.createChannel_Gesture_Kinect("Gestur_Kinect", "amq.topic", "gesture.kinect");
            rabbitmq.CreateRMQChannel2("modenomorkelas", "amq.topic", "kondisiruang.modekelas");
            rabbitmq.CreateRMQChannel2("Aktuator", "amq.topic", "kondisiruang.lampu1");
            rabbitmq.CreateRMQChannel2("Aktuator", "amq.topic", "kondisiruang.lampu2");
            rabbitmq.CreateRMQChannel2("Aktuator", "amq.topic", "kondisiruang.lampu3");
            rabbitmq.CreateRMQChannel2("Aktuator", "amq.topic", "kondisiruang.lampu4");
            rabbitmq.CreateRMQChannel2("Aktuator", "amq.topic", "kondisiruang.kipas1");
            rabbitmq.CreateRMQChannel2("Aktuator", "amq.topic", "kondisiruang.kipas2");

            // Deklarasi Nilai Awal Posisi dan Ukuran Node
            temp.x = 0; temp.y = 0; temp.z = 1; temp.skala = 0; temp.r = 0;
            hum.x = 0; hum.y = -0.3f; hum.z = 1; hum.skala = 0; hum.r = 0;
            intens.x = 0.4f; intens.y = 0; intens.z = 1; intens.skala = 0; intens.r = 0;
            noise.x = 0.4f; noise.y = -0.3f; noise.z = 1; noise.skala = 0; noise.r = 0;
            lampu.x = 0.2f; lampu.y = 0.1f; lampu.z = 1; lampu.skala = 0; lampu.r = 0;
            kipas.x = 0.2f; kipas.y = -0.2f; kipas.z = 1; kipas.skala = 0; kipas.r = 0;
            kotak.x = 0; kotak.y = -0.1f; kotak.z = 1; kotak.skala = 0.005f; kotak.r = 0;
            jarak.x = 0; jarak.y = 0; jarak.z = 1; jarak.skala = 0; jarak.r = 0;

            // Deklarasi Nilai Awal Node Pemilihan Mode Kelas
            kelas.x = 0.2f; kelas.y = -0.1f; kelas.z = 1; kelas.skala = 0;

            //Deklarasi Nilai Aktuator dan Slider Kontrol
            nlampu1 = 1000; nlampu2 = 1000; nlampu3 = 2000; nlampu4 = 2000; nkipas1 = 0; nkipas2 = 0;
            njarak1 = 0; njarak2 = 0; njarak3 = 0;
            slider1plus.x = -0.8f; slider1plus.y = -1.57f; slider1plus.z = 0;
            slider1min.x = -0.8f; slider1min.y = -1.57f; slider1min.z = 0;
            slider2plus.x = -0.3f; slider2plus.y = -1.57f; slider2plus.z = 0;
            slider2min.x = -0.3f; slider2min.y = -1.57f; slider2min.z = 0;
            slider3plus.x = 0.2f; slider3plus.y = -1.57f; slider3plus.z = 0;
            slider3min.x = 0.2f; slider3min.y = -1.57f; slider3min.z = 0;
            slider4plus.x = 0.7f; slider4plus.y = -1.57f; slider4plus.z = 0;
            slider4min.x = 0.7f; slider4min.y = -1.57f; slider4min.z = 0;
            slider5plus.x = -0.8f; slider5plus.y = -1.57f; slider5plus.z = 0;
            slider5min.x = -0.8f; slider5min.y = -1.57f; slider5min.z = 0;
            slider6plus.x = -0.3f; slider6plus.y = -1.57f; slider6plus.z = 0;
            slider6min.x = -0.3f; slider6min.y = -1.57f; slider6min.z = 0;

            // Node Untuk Tampilan Awal
            tampilanPilihanMode();
            tampilanSuhu();
            tampilanKelembapan();
            tampilanIntensitasCahaya();
            tampilanKebisingan();
            tampilanKontrolLampu();
            tampilanKontrolKipas();
            tampilanPilihanKelas();
            tampilanKontrolJarak();

            // Kondisi Awal UI
            ui = new UITwoD(ref uiRoot);
            ui_4Sensor = new UITwoD_4Sensor(ref uiRoot);
            ui_Voice = new UITwoD_Voice(ref uiRoot);
            ui_Face = new UITwoD_Face(ref uiRoot);
            ui_Gesture = new UITwoD_Gesture(ref uiRoot);

            ui.eraseUI();
            ui_4Sensor.eraseUI();
            ui_Voice.eraseUI();
            ui_Face.eraseUI();
            ui_Gesture.eraseUI();


            KondisiRuang_2D = false;
            KondisiRuang_3D = false;
            KondisiRuang_4Sensor = false;
            VoiceCommand_2D = false;
            Gesture_2D = false;
            Face_2D = false;
            JarakSensor = false;

        }

        // For HL optical stabilization (optional)
        public override Vector3 FocusWorldPoint => suhuNode.WorldPosition;

        // Fungsi Update Time dan Reeceive Data
        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            var dataRMQ = "";

            var dataRMQ2 = "";

            var dataRMQ3 = "";

            var dataRMQ4 = "";

            var dataRMQ5 = "";

            if (rabbitmq.getData() != "")
            {
                try
                {
                    dataJson = JsonConvert.DeserializeObject<sensormessage>(rabbitmq.getData());
                    dataRMQ = rabbitmq.getData();
                    // Menampilkan Gambar dan Nilai Suhu
                    GambarSuhu(dataJson.temperature);
                    NilaiSuhu(dataJson.temperature);
                    textRekSuhu(dataJson.recomtemp);
                    // speakerText.Value = "Nama : " + dataJson2.nama;

                    // Menampilkan Gambar dan Nilai Kelembapan
                    GambarKelembapan(dataJson.humidity);
                    NilaiKelembapan(dataJson.humidity);
                    textRekHum(dataJson.recomhumid);

                    // Menampilkan Gambar dan Nilai Intensitas Cahaya
                    GambarIntensitas(dataJson.lightintensity);
                    NilaiIntensitas(dataJson.lightintensity);
                    textRekIntens(dataJson.recomlight);

                    // Menampilkan Gambar dan Nilai Kebisingan
                    GambarKebisingan(dataJson.soundlevel);
                    NilaiKebisingan(dataJson.soundlevel);
                    textRekNoise(dataJson.recomsound);

                    if (KondisiRuang_2D == true)
                    {
                        ui.updateUI(dataJson.temperature, dataJson.humidity, dataJson.lightintensity, dataJson.soundlevel, nmode);
                    }

                    if (KondisiRuang_4Sensor == true)
                    {
                        ui_4Sensor.updateUI(dataJson.temperature, dataJson.humidity, dataJson.lightintensity, dataJson.soundlevel, dataJson.sensor_id, nmode);
                    }
                }
                catch (JsonSerializationException jsonSerializerEx)
                {
                    Debug.WriteLine("Json Serialization Exception : " + jsonSerializerEx.Message.ToString());
                }
                catch (JsonReaderException jsonReaderEx)
                {
                    Debug.WriteLine("Json Reader Exception : " + jsonReaderEx.Message.ToString());

                }
                catch (JsonException jsonEx)
                {
                    Debug.WriteLine("Json Exception : " + jsonEx.Message.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception : " + ex.Message.ToString());
                }
            }

            if (rabbitmq.getData_Voice() != "")
            {
                try
                {
                    dataJson2 = JsonConvert.DeserializeObject<voicemessage>(rabbitmq.getData_Voice());
                    dataRMQ2 = rabbitmq.getData_Voice();

                    //Menampilkan Tampilan Voice Command
                    if (VoiceCommand_PerintahDiterima != dataJson2.perintah)
                    {
                        VoiceCommand_Update(dataJson2.nama, dataJson2.perintah);
                        
                    }
                    if (dataJson2.perintah != "ian\n" && dataJson2.perintah != "\n")
                    {
                        ui_Voice.updateUI(dataJson2.nama, dataJson2.perintah, dataJson2.jarak);
                    }
                    VoiceCommand_PerintahDiterima = dataJson2.perintah;
                }
                catch (JsonSerializationException jsonSerializerEx)
                {
                    Debug.WriteLine("Json Serialization Exception : " + jsonSerializerEx.Message.ToString());
                }
                catch (JsonReaderException jsonReaderEx)
                {
                    Debug.WriteLine("Json Reader Exception : " + jsonReaderEx.Message.ToString());

                }
                catch (JsonException jsonEx)
                {
                    Debug.WriteLine("Json Exception : " + jsonEx.Message.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception : " + ex.Message.ToString());
                }

            }

            if (rabbitmq.getData_Face() != "")
            {
                try
                {
                    dataJson3 = JsonConvert.DeserializeObject<facemessage>(rabbitmq.getData_Face());
                    dataRMQ3 = rabbitmq.getData_Face();

                    //Menampilkan Tampilan Data Face
                    if (dataJson3.ekspresi != null && Face_2D == true)
                    {
                        ui_Face.updateUI(dataJson3.nama, dataJson3.ekspresi);
                    }
                }
                catch (JsonSerializationException jsonSerializerEx)
                {
                    Debug.WriteLine("Json Serialization Exception : " + jsonSerializerEx.Message.ToString());
                }
                catch (JsonReaderException jsonReaderEx)
                {
                    Debug.WriteLine("Json Reader Exception : " + jsonReaderEx.Message.ToString());

                }
                catch (JsonException jsonEx)
                {
                    Debug.WriteLine("Json Exception : " + jsonEx.Message.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception : " + ex.Message.ToString());
                }
            }

            if (rabbitmq.getData_Gesture() != "")
            {
                try
                {
                    dataJson4 = JsonConvert.DeserializeObject<gesturemessage>(rabbitmq.getData_Gesture());
                    dataRMQ4 = rabbitmq.getData_Gesture();

                    //Menampilkan Tampilan Data Face
                    if (dataJson4.gesture  != null && Gesture_2D == true )
                    {
                        ui_Gesture.updateUI(dataJson4.cameraID, dataJson4.gesture, dataJson4.confidence);
                    }
                }
                catch (JsonSerializationException jsonSerializerEx)
                {
                    Debug.WriteLine("Json Serialization Exception : " + jsonSerializerEx.Message.ToString());
                }
                catch (JsonReaderException jsonReaderEx)
                {
                    Debug.WriteLine("Json Reader Exception : " + jsonReaderEx.Message.ToString());

                }
                catch (JsonException jsonEx)
                {
                    Debug.WriteLine("Json Exception : " + jsonEx.Message.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception : " + ex.Message.ToString());
                }
            }

            if (rabbitmq.getData_Gesture_Kinect() != "")
            {
                try
                {
                    dataRMQ5 = rabbitmq.getData_Gesture_Kinect();
                    
                    //Menampilkan Tampilan Data Face
                    if (dataRMQ5 != null && Gesture_2D == true)
                    {
                        ui_Gesture.updateUI_Kinect(dataRMQ5);
                    }
                }
                catch (JsonSerializationException jsonSerializerEx)
                {
                    Debug.WriteLine("Json Serialization Exception : " + jsonSerializerEx.Message.ToString());
                }
                catch (JsonReaderException jsonReaderEx)
                {
                    Debug.WriteLine("Json Reader Exception : " + jsonReaderEx.Message.ToString());

                }
                catch (JsonException jsonEx)
                {
                    Debug.WriteLine("Json Exception : " + jsonEx.Message.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception : " + ex.Message.ToString());
                }
            }

        }

        public void kirimpesan(int a)
        {
            if (a == 1)
            {
                rabbitmq.SendMessage("amq.topic", "kondisiruang.lampu1", nlampu1.ToString());
            }
            else if (a == 2)
            {
                rabbitmq.SendMessage("amq.topic", "kondisiruang.lampu2", nlampu2.ToString());
            }
            else if (a == 3)
            {
                rabbitmq.SendMessage("amq.topic", "kondisiruang.lampu3", nlampu3.ToString());
            }
            else if (a == 4)
            {
                rabbitmq.SendMessage("amq.topic", "kondisiruang.lampu4", nlampu4.ToString());
            }
            else if (a == 5)
            {
                rabbitmq.SendMessage("amq.topic", "kondisiruang.kipas1", nkipas1.ToString());
            }
            else
            {
                rabbitmq.SendMessage("amq.topic", "kondisiruang.kipas2", nkipas2.ToString());
            }
        }

        public void kirimpesan_modekelas()
        {
            ////Buat format json
            //modekelas account = new modekelas
            //{
            //    mode = nmode
            //};

            //string sendjson = JsonConvert.SerializeObject(account, Formatting.Indented);

            rabbitmq.SendMessage("amq.topic", "kondisiruang.modekelas", nmode.ToString());
            //Debug.WriteLine("Send Message" + sendjson);
        }

        // Tampilan Pilihan
        public void tampilanPilihanMode()
        {
            Kotak3D = Scene.CreateChild("Kotak3D");
            Kotak3D.Position = new Vector3(kotak.x, kotak.y, kotak.z);
            Kotak3D.SetScale(kotak.skala);
            //suhuNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 0);
            var Box3D = Kotak3D.CreateComponent<StaticModel>();
            Box3D.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            Box3D.SetMaterial(ResourceCache.GetMaterial("Materials/intemp3.xml"));

            JudulHoloSensor = Kotak3D.CreateChild("Teks3D");
            var JudulHolo = JudulHoloSensor.CreateComponent<Text3D>();
            JudulHoloSensor.Position = new Vector3(-2 , 7, 0);
            JudulHolo.HorizontalAlignment = HorizontalAlignment.Center;
            JudulHolo.VerticalAlignment = VerticalAlignment.Top;
            JudulHoloSensor.SetScale(3);
            // Pilihan3D.ViewMask = 0x80000000; //hide from raycasts
            JudulHolo.Text = "HoloSensor";
            JudulHolo.SetFont(CoreAssets.Fonts.AnonymousPro, 1000);
            JudulHolo.SetColor(Color.White);

            Teks3D = Kotak3D.CreateChild("Teks3D");
            var Pilihan3D = Teks3D.CreateComponent<Text3D>();
            Teks3D.Position = new Vector3(6, 3, 0);
            Pilihan3D.HorizontalAlignment = HorizontalAlignment.Center;
            Pilihan3D.VerticalAlignment = VerticalAlignment.Top;
            Teks3D.SetScale(1);
            // Pilihan3D.ViewMask = 0x80000000; //hide from raycasts
            Pilihan3D.Text = "Mode 3 Dimensi";
            Pilihan3D.SetFont(CoreAssets.Fonts.AnonymousPro, 300);
            Pilihan3D.SetColor(Color.White);

            Kotak2D = Kotak3D.CreateChild("Kotak2D");
            Kotak2D.Position = new Vector3(0, -5, 0);
            Kotak2D.SetScale(1);
            //suhuNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 0);
            var Box2D = Kotak2D.CreateComponent<StaticModel>();
            Box2D.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            Box2D.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));

            Teks2D = Kotak3D.CreateChild("Teks2D");
            var Pilihan2D = Teks2D.CreateComponent<Text3D>();
            Teks2D.Position = new Vector3(6, -3, 0);
            Teks2D.SetScale(1);
            Pilihan2D.HorizontalAlignment = HorizontalAlignment.Center;
            Pilihan2D.VerticalAlignment = VerticalAlignment.Top;
            // Pilihan2D.ViewMask = 0x80000000; //hide from raycasts
            Pilihan2D.Text = "Mode 2 Dimensi";
            Pilihan2D.SetFont(CoreAssets.Fonts.AnonymousPro, 300);
            Pilihan2D.SetColor(Color.White);

            Kotak4Sensor = Kotak3D.CreateChild("Kotak4Sensor");
            Kotak4Sensor.Position = new Vector3(-15, 0, 0);
            Kotak4Sensor.SetScale(1);
            var Box4Sensor = Kotak4Sensor.CreateComponent<StaticModel>();
            Box4Sensor.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            Box4Sensor.SetMaterial(Material.FromColor(Color.Yellow));

            Teks4Sensor = Kotak3D.CreateChild("Teks4Sensor");
            var Pilihan4Sensor = Teks4Sensor.CreateComponent<Text3D>();
            Teks4Sensor.Position = new Vector3(-9, 3, 0);
            Teks4Sensor.SetScale(1);
            Pilihan4Sensor.HorizontalAlignment = HorizontalAlignment.Center;
            Pilihan4Sensor.VerticalAlignment = VerticalAlignment.Top;
            // Pilihan4Sensor.ViewMask = 0x80000000; //hide from raycasts
            Pilihan4Sensor.Text = "   Tampilan 4 Sensor";
            Pilihan4Sensor.SetFont(CoreAssets.Fonts.AnonymousPro, 300);
            Pilihan4Sensor.SetColor(Color.White);


            KotakJarak = Kotak3D.CreateChild("KotakJarak");
            KotakJarak.Position = new Vector3(-15, -5, 0);
            KotakJarak.SetScale(1);
            //suhuNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 0);
            var BoxJarak = KotakJarak.CreateComponent<StaticModel>();
            BoxJarak.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            BoxJarak.SetMaterial(Material.FromColor(Color.Blue));

            TeksJarak = Kotak3D.CreateChild("TeksJarak");
            var PilihanJarak = TeksJarak.CreateComponent<Text3D>();
            TeksJarak.Position = new Vector3(-10, -3, 0);
            TeksJarak.SetScale(1);
            PilihanJarak.HorizontalAlignment = HorizontalAlignment.Center;
            PilihanJarak.VerticalAlignment = VerticalAlignment.Top;
            // PilihanJarak.ViewMask = 0x80000000; //hide from raycasts
            PilihanJarak.Text = "Jarak Sensor";
            PilihanJarak.SetFont(CoreAssets.Fonts.AnonymousPro, 300);
            PilihanJarak.SetColor(Color.White);

        }

        // Tampilan Awal
        public void tampilanSuhu()
        {
            suhuNode = Scene.CreateChild("Temperature");
            suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
            suhuNode.SetScale(temp.skala);
            //suhuNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 0);
            var suhu = suhuNode.CreateComponent<StaticModel>();
            suhu.Model = ResourceCache.GetModel("Models/intemp4.mdl");
            suhu.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));
            suhu2Node = suhuNode.CreateChild("Temperature");
            suhu2Node.Position = new Vector3(temp.x, temp.y, (temp.z - 1));
            var suhu2 = suhu2Node.CreateComponent<StaticModel>();
            suhu2.Model = ResourceCache.GetModel("Models/outtemp.mdl");
            suhu2.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            textTemp = suhuNode.CreateChild("textTemp");
            var text3D1 = textTemp.CreateComponent<Text3D>();
            textTemp.Position = new Vector3(0, -0.1f, 0);
            text3D1.HorizontalAlignment = HorizontalAlignment.Center;
            text3D1.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D1.Text = "Suhu Ruang :";
            text3D1.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D1.SetColor(Color.White);
            //textTemp.Translate(new Vector3(0, -0.1f, 0));

            textTempRek = suhuNode.CreateChild("textTempRek");
            var text3D1R = textTempRek.CreateComponent<Text3D>();
            textTempRek.Position = new Vector3(0, -0.3f, 0);
            text3D1R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D1R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D1R.Text = "Rekomendasi : ";
            text3D1R.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D1R.SetColor(Color.White);
            //textTempRek.Translate(new Vector3(0, -0.3f, 0));

            // Kontrol Posisi Node Suhu
            upsuhu = suhuNode.CreateChild("upsuhu");
            upsuhu.SetScale(0.5F);
            upsuhu.Position = new Vector3(0.9f, 3.2f, 0);
            upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up1 = upsuhu.CreateComponent<StaticModel>();
            up1.Model = ResourceCache.GetModel("Models/panah.mdl");
            up1.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            rightsuhu = suhuNode.CreateChild("rightsuhu");
            rightsuhu.SetScale(0.5F);
            rightsuhu.Position = new Vector3(1, 3.1f, 0);
            rightsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right1 = rightsuhu.CreateComponent<StaticModel>();
            right1.Model = ResourceCache.GetModel("Models/panah.mdl");
            right1.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            downsuhu = suhuNode.CreateChild("downsuhu");
            downsuhu.SetScale(0.5F);
            downsuhu.Position = new Vector3(0.9f, 3, 0);
            downsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down1 = downsuhu.CreateComponent<StaticModel>();
            down1.Model = ResourceCache.GetModel("Models/panah.mdl");
            down1.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            leftsuhu = suhuNode.CreateChild("leftsuhu");
            leftsuhu.SetScale(0.5F);
            leftsuhu.Position = new Vector3(0.8f, 3.1f, 0);
            leftsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left1 = leftsuhu.CreateComponent<StaticModel>();
            left1.Model = ResourceCache.GetModel("Models/panah.mdl");
            left1.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            frontsuhu = suhuNode.CreateChild("frontsuhu");
            frontsuhu.SetScale(0.5F);
            frontsuhu.Position = new Vector3(-0.9f, 3.2f, 0);
            frontsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front1 = frontsuhu.CreateComponent<StaticModel>();
            front1.Model = ResourceCache.GetModel("Models/panah.mdl");
            front1.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backsuhu = suhuNode.CreateChild("backsuhu");
            backsuhu.SetScale(0.5F);
            backsuhu.Position = new Vector3(-0.9f, 3, 0);
            backsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back1 = backsuhu.CreateComponent<StaticModel>();
            back1.Model = ResourceCache.GetModel("Models/panah.mdl");
            back1.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Suhu
            plussuhu = suhuNode.CreateChild("plussuhu");
            plussuhu.SetScale(0.05F);
            plussuhu.Position = new Vector3(0.9f, 2.3f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus1 = plussuhu.CreateComponent<StaticModel>();
            plus1.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus1.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bplussuhu = suhuNode.CreateChild("plussuhu");
            bplussuhu.SetScale(0.05F);
            bplussuhu.Position = new Vector3(0.9f, 2.3f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus1 = bplussuhu.CreateComponent<StaticModel>();
            bplus1.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus1.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minsuhu = suhuNode.CreateChild("minsuhu");
            minsuhu.SetScale(0.05F);
            minsuhu.Position = new Vector3(0.9f, 2, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min1 = minsuhu.CreateComponent<StaticModel>();
            min1.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min1.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminsuhu = suhuNode.CreateChild("minsuhu");
            bminsuhu.SetScale(0.05F);
            bminsuhu.Position = new Vector3(0.9f, 2, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin1 = bminsuhu.CreateComponent<StaticModel>();
            bmin1.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin1.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            // Kontrol Rotasi Node Suhu
            rotate_pos_suhu = suhuNode.CreateChild("rotate_pos_suhu");
            rotate_pos_suhu.SetScale(0.5F);
            rotate_pos_suhu.Position = new Vector3(-0.8f, 1.4f, 0);
            //rotate_pos_suhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 0), 0);
            var rotasi_pos_suhu = rotate_pos_suhu.CreateComponent<StaticModel>();
            rotasi_pos_suhu.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_pos_suhu.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            rotate_neg_suhu = suhuNode.CreateChild("rotate_neg_suhu");
            rotate_neg_suhu.SetScale(0.5F);
            rotate_neg_suhu.Position = new Vector3(0.8f, 1.4f, 0);
            rotate_neg_suhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 180);
            var rotasi_neg_suhu = rotate_neg_suhu.CreateComponent<StaticModel>();
            rotasi_neg_suhu.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_neg_suhu.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));
        }

        public void tampilanKelembapan()
        {
            // Node untuk Kelembapan
            lembapNode = Scene.CreateChild("Humidity");
            lembapNode.SetScale(hum.skala);
            lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
            var lembap = lembapNode.CreateComponent<StaticModel>();
            lembap.Model = ResourceCache.GetModel("Models/hum4.mdl");
            lembap.SetMaterial(ResourceCache.GetMaterial("Materials/hum4.xml"));

            textHum = lembapNode.CreateChild("textHum");
            var text3D2 = textHum.CreateComponent<Text3D>();
            textHum.Position = new Vector3(0, -0.1f, 0);
            text3D2.HorizontalAlignment = HorizontalAlignment.Center;
            text3D2.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D2.Text = "Kelembapan Ruang :";
            text3D2.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D2.SetColor(Color.White);
            //textHum.Translate(new Vector3(0, -0.1f, 0));

            textHumRek = lembapNode.CreateChild("textHumRek");
            var text3D2R = textHumRek.CreateComponent<Text3D>();
            textHumRek.Position = new Vector3(0, -0.3f, 0);
            text3D2R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D2R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D2R.Text = "Rekomendasi : ";
            text3D2R.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D2R.SetColor(Color.White);
            //textHumRek.Translate(new Vector3(0, -0.3f, 0));


            // Kontrol Posisi Node Kelembapan
            uphum = lembapNode.CreateChild("uphum");
            uphum.SetScale(0.5F);
            uphum.Position = new Vector3(0.9f, 3.2f, 0);
            uphum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up2 = uphum.CreateComponent<StaticModel>();
            up2.Model = ResourceCache.GetModel("Models/panah.mdl");
            up2.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            righthum = lembapNode.CreateChild("righthum");
            righthum.SetScale(0.5F);
            righthum.Position = new Vector3(1, 3.1f, 0);
            righthum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right2 = righthum.CreateComponent<StaticModel>();
            right2.Model = ResourceCache.GetModel("Models/panah.mdl");
            right2.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            downhum = lembapNode.CreateChild("downhum");
            downhum.SetScale(0.5F);
            downhum.Position = new Vector3(0.9f, 3, 0);
            downhum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down2 = downhum.CreateComponent<StaticModel>();
            down2.Model = ResourceCache.GetModel("Models/panah.mdl");
            down2.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            lefthum = lembapNode.CreateChild("lefthum");
            lefthum.SetScale(0.5F);
            lefthum.Position = new Vector3(0.8f, 3.1f, 0);
            lefthum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left2 = lefthum.CreateComponent<StaticModel>();
            left2.Model = ResourceCache.GetModel("Models/panah.mdl");
            left2.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            fronthum = lembapNode.CreateChild("fronthum");
            fronthum.SetScale(0.5F);
            fronthum.Position = new Vector3(-0.9f, 3.2f, 0);
            fronthum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front2 = fronthum.CreateComponent<StaticModel>();
            front2.Model = ResourceCache.GetModel("Models/panah.mdl");
            front2.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backhum = lembapNode.CreateChild("backhum");
            backhum.SetScale(0.5F);
            backhum.Position = new Vector3(-0.9f, 3, 0);
            backhum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back2 = backhum.CreateComponent<StaticModel>();
            back2.Model = ResourceCache.GetModel("Models/panah.mdl");
            back2.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Kelembapan
            plushum = lembapNode.CreateChild("plushum");
            plushum.SetScale(0.05F);
            plushum.Position = new Vector3(0, 3.4f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus2 = plushum.CreateComponent<StaticModel>();
            plus2.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus2.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bplushum = lembapNode.CreateChild("plushum");
            bplushum.SetScale(0.05F);
            bplushum.Position = new Vector3(0, 3.4f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus2 = bplushum.CreateComponent<StaticModel>();
            bplus2.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus2.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minhum = lembapNode.CreateChild("minhum");
            minhum.SetScale(0.05F);
            minhum.Position = new Vector3(0, 3.1f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min2 = minhum.CreateComponent<StaticModel>();
            min2.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min2.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminhum = lembapNode.CreateChild("minhum");
            bminhum.SetScale(0.05F);
            bminhum.Position = new Vector3(0, 3.1f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin2 = bminhum.CreateComponent<StaticModel>();
            bmin2.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin2.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            // Kontrol Rotasi
            // Kontrol Rotasi Node Suhu
            rotate_pos_hum = lembapNode.CreateChild("rotate_pos_hum");
            rotate_pos_hum.SetScale(0.5F);
            rotate_pos_hum.Position = new Vector3(-1.2f, 1.4f, 0);
            //rotate_pos_hum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 0), 0);
            var rotasi_pos_hum = rotate_pos_hum.CreateComponent<StaticModel>();
            rotasi_pos_hum.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_pos_hum.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            rotate_neg_hum = lembapNode.CreateChild("rotate_neg_hum");
            rotate_neg_hum.SetScale(0.5F);
            rotate_neg_hum.Position = new Vector3(1.2f, 1.4f, 0);
            rotate_neg_hum.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 180);
            var rotasi_neg_hum = rotate_neg_hum.CreateComponent<StaticModel>();
            rotasi_neg_hum.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_neg_hum.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));
        }

        public void tampilanIntensitasCahaya()
        {
            // Node untuk Intensitas Cahaya
            cahayaNode = Scene.CreateChild("Light Intensity");
            cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
            cahayaNode.SetScale(intens.skala);
            var cahaya = cahayaNode.CreateComponent<StaticModel>();
            cahaya.Model = ResourceCache.GetModel("Models/bohlam4.mdl");
            cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light4.xml"));
            cahaya2Node = cahayaNode.CreateChild("Light Intensity");
            cahaya2Node.Position = new Vector3(0, 0, 0);
            var cahaya2 = cahaya2Node.CreateComponent<StaticModel>();
            cahaya2.Model = ResourceCache.GetModel("Models/tengah.mdl");
            cahaya2.SetMaterial(ResourceCache.GetMaterial("Materials/tengah.xml"));
            cahaya3Node = cahayaNode.CreateChild("Light Intensity");
            cahaya3Node.Position = new Vector3(0, 0, 0);
            var cahaya3 = cahaya3Node.CreateComponent<StaticModel>();
            cahaya3.Model = ResourceCache.GetModel("Models/bawah.mdl");
            cahaya3.SetMaterial(ResourceCache.GetMaterial("Materials/bawah.xml"));

            textIntens = cahayaNode.CreateChild("textIntens");
            var text3D3 = textIntens.CreateComponent<Text3D>();
            textIntens.Position = new Vector3(0, -0.1f, 0);
            text3D3.HorizontalAlignment = HorizontalAlignment.Center;
            text3D3.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D3.Text = "Intensitas Cahaya Ruang :";
            text3D3.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D3.SetColor(Color.White);
            //textIntens.Translate(new Vector3(0, -0.1f, 0));

            textIntensRek = cahayaNode.CreateChild("textIntensRek");
            var text3D3R = textIntensRek.CreateComponent<Text3D>();
            textIntensRek.Position = new Vector3(0, -0.3f, 0);
            text3D3R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D3R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D3R.Text = "Rekomendasi : ";
            text3D3R.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D3R.SetColor(Color.White);
            //textIntensRek.Translate(new Vector3(0, -0.3f, 0));

            // Kontrol Posisi Node Kelembapan
            upchy = cahayaNode.CreateChild("upchy");
            upchy.SetScale(0.5F);
            upchy.Position = new Vector3(0.9f, 1.4f, 0);
            upchy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up3 = upchy.CreateComponent<StaticModel>();
            up3.Model = ResourceCache.GetModel("Models/panah.mdl");
            up3.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            rightchy = cahayaNode.CreateChild("rightchy");
            rightchy.SetScale(0.5F);
            rightchy.Position = new Vector3(1, 1.3f, 0);
            rightchy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right3 = rightchy.CreateComponent<StaticModel>();
            right3.Model = ResourceCache.GetModel("Models/panah.mdl");
            right3.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            downchy = cahayaNode.CreateChild("downchy");
            downchy.SetScale(0.5F);
            downchy.Position = new Vector3(0.9f, 1.2f, 0);
            downchy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down3 = downchy.CreateComponent<StaticModel>();
            down3.Model = ResourceCache.GetModel("Models/panah.mdl");
            down3.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            leftchy = cahayaNode.CreateChild("leftchy");
            leftchy.SetScale(0.5F);
            leftchy.Position = new Vector3(0.8f, 1.3f, 0);
            leftchy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left3 = leftchy.CreateComponent<StaticModel>();
            left3.Model = ResourceCache.GetModel("Models/panah.mdl");
            left3.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            frontchy = cahayaNode.CreateChild("frontchy");
            frontchy.SetScale(0.5F);
            frontchy.Position = new Vector3(-1.1f, 1.4f, 0);
            frontchy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front3 = frontchy.CreateComponent<StaticModel>();
            front3.Model = ResourceCache.GetModel("Models/panah.mdl");
            front3.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backchy = cahayaNode.CreateChild("backchy");
            backchy.SetScale(0.5F);
            backchy.Position = new Vector3(-1.1f, 1.2f, 0);
            backchy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back3 = backchy.CreateComponent<StaticModel>();
            back3.Model = ResourceCache.GetModel("Models/panah.mdl");
            back3.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Intensitas Cahaya
            pluschy = cahayaNode.CreateChild("pluschy");
            pluschy.SetScale(0.05F);
            pluschy.Position = new Vector3(0.9f, 0.5f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus3 = pluschy.CreateComponent<StaticModel>();
            plus3.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus3.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bpluschy = cahayaNode.CreateChild("pluschy");
            bpluschy.SetScale(0.05F);
            bpluschy.Position = new Vector3(0.9f, 0.5f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus3 = bpluschy.CreateComponent<StaticModel>();
            bplus3.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus3.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minchy = cahayaNode.CreateChild("minchy");
            minchy.SetScale(0.05F);
            minchy.Position = new Vector3(0.9f, 0.2f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min3 = minchy.CreateComponent<StaticModel>();
            min3.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min3.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminchy = cahayaNode.CreateChild("minchy");
            bminchy.SetScale(0.05F);
            bminchy.Position = new Vector3(0.9f, 0.2f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin3 = bminchy.CreateComponent<StaticModel>();
            bmin3.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin3.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            // Kontrol Rotasi
            // Kontrol Rotasi Node Suhu
            rotate_pos_chy = cahayaNode.CreateChild("rotate_pos_chy");
            rotate_pos_chy.SetScale(0.5F);
            rotate_pos_chy.Position = new Vector3(-0.7f, 0.2f, 0);
            //rotate_pos_chy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 0), 0);
            var rotasi_pos_chy = rotate_pos_chy.CreateComponent<StaticModel>();
            rotasi_pos_chy.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_pos_chy.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            rotate_neg_chy = cahayaNode.CreateChild("rotate_neg_chy");
            rotate_neg_chy.SetScale(0.5F);
            rotate_neg_chy.Position = new Vector3(0.5f, 0.2f, 0);
            rotate_neg_chy.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 180);
            var rotasi_neg_chy = rotate_neg_chy.CreateComponent<StaticModel>();
            rotasi_neg_chy.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_neg_chy.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));
        }

        public void tampilanKebisingan()
        {
            //// Node untuk Kebisingan
            bisingNode = Scene.CreateChild("Noise");
            bisingNode.SetScale(noise.skala);
            bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
            var bising = bisingNode.CreateComponent<StaticModel>();
            bising.Model = ResourceCache.GetModel("Models/noise3.mdl");
            bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise3.xml"));

            textNoise = bisingNode.CreateChild("textNoise");
            var text3D4 = textNoise.CreateComponent<Text3D>();
            textNoise.Position = new Vector3(0, -0.1f, 0);
            text3D4.HorizontalAlignment = HorizontalAlignment.Center;
            text3D4.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D4.Text = "Kebisingan Ruang :";
            text3D4.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D4.SetColor(Color.White);
            //textNoise.Translate(new Vector3(0, -0.1f, 0));

            textNoiseRek = bisingNode.CreateChild("textNoiseRek");
            var text3D4R = textNoiseRek.CreateComponent<Text3D>();
            textNoiseRek.Position = new Vector3(0, -0.3f, 0);
            text3D4R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D4R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D4R.Text = "Rekomendasi : ";
            text3D4R.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            text3D4R.SetColor(Color.White);
            //textNoiseRek.Translate(new Vector3(0, -0.3f, 0));

            // Kontrol Posisi Node Noise
            upnoise = bisingNode.CreateChild("upnoise");
            upnoise.SetScale(0.5F);
            upnoise.Position = new Vector3(0.9f, 3.2f, 0);
            upnoise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up4 = upnoise.CreateComponent<StaticModel>();
            up4.Model = ResourceCache.GetModel("Models/panah.mdl");
            up4.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            rightnoise = bisingNode.CreateChild("rightnoise");
            rightnoise.SetScale(0.5F);
            rightnoise.Position = new Vector3(1, 3.1f, 0);
            rightnoise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right4 = rightnoise.CreateComponent<StaticModel>();
            right4.Model = ResourceCache.GetModel("Models/panah.mdl");
            right4.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            downnoise = bisingNode.CreateChild("downnoise");
            downnoise.SetScale(0.5F);
            downnoise.Position = new Vector3(0.9f, 3, 0);
            downnoise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down4 = downnoise.CreateComponent<StaticModel>();
            down4.Model = ResourceCache.GetModel("Models/panah.mdl");
            down4.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            leftnoise = bisingNode.CreateChild("leftnoise");
            leftnoise.SetScale(0.5F);
            leftnoise.Position = new Vector3(0.8f, 3.1f, 0);
            leftnoise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left4 = leftnoise.CreateComponent<StaticModel>();
            left4.Model = ResourceCache.GetModel("Models/panah.mdl");
            left4.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            frontnoise = bisingNode.CreateChild("frontnoise");
            frontnoise.SetScale(0.5F);
            frontnoise.Position = new Vector3(-0.9f, 3.2f, 0);
            frontnoise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front4 = frontnoise.CreateComponent<StaticModel>();
            front4.Model = ResourceCache.GetModel("Models/panah.mdl");
            front4.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backnoise = bisingNode.CreateChild("backnoise");
            backnoise.SetScale(0.5F);
            backnoise.Position = new Vector3(-0.9f, 3, 0);
            backnoise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back4 = backnoise.CreateComponent<StaticModel>();
            back4.Model = ResourceCache.GetModel("Models/panah.mdl");
            back4.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Noise
            plusnoise = bisingNode.CreateChild("plusnoise");
            plusnoise.SetScale(0.05F);
            plusnoise.Position = new Vector3(0.9f, 2.3f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus4 = plusnoise.CreateComponent<StaticModel>();
            plus4.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus4.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bplusnoise = bisingNode.CreateChild("plusnoise");
            bplusnoise.SetScale(0.05F);
            bplusnoise.Position = new Vector3(0.9f, 2.3f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus4 = bplusnoise.CreateComponent<StaticModel>();
            bplus4.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus4.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minnoise = bisingNode.CreateChild("minnoise");
            minnoise.SetScale(0.05F);
            minnoise.Position = new Vector3(0.9f, 2, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min4 = minnoise.CreateComponent<StaticModel>();
            min4.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min4.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminnoise = bisingNode.CreateChild("minnoise");
            bminnoise.SetScale(0.05F);
            bminnoise.Position = new Vector3(0.9f, 2, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin4 = bminnoise.CreateComponent<StaticModel>();
            bmin4.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin4.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            // Kontrol Rotasi
            // Kontrol Rotasi Node Suhu
            rotate_pos_noise = bisingNode.CreateChild("rotate_pos_noise");
            rotate_pos_noise.SetScale(0.5F);
            rotate_pos_noise.Position = new Vector3(-1.3f, 2.1f, 0);
            //rotate_pos_noise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 0), 0);
            var rotasi_pos_noise = rotate_pos_noise.CreateComponent<StaticModel>();
            rotasi_pos_noise.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_pos_noise.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            rotate_neg_noise = bisingNode.CreateChild("rotate_neg_noise");
            rotate_neg_noise.SetScale(0.5F);
            rotate_neg_noise.Position = new Vector3(1.3f, 2.1f, 0);
            rotate_neg_noise.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 180);
            var rotasi_neg_noise = rotate_neg_noise.CreateComponent<StaticModel>();
            rotasi_neg_noise.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_neg_noise.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));
        }

        // Fungsi Mengontrol Aktuator
        public void tampilanKontrolLampu()
        {
            textjudullampu = Scene.CreateChild("Kontrol Lampu");
            textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
            textjudullampu.SetScale(lampu.skala);
            var textlampu = textjudullampu.CreateComponent<Text3D>();
            textlampu.HorizontalAlignment = HorizontalAlignment.Center;
            textlampu.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textlampu.Text = "Kontrol Lampu";
            textlampu.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textlampu.SetColor(Color.Blue);
            //textNoise.Translate(new Vector3(0, -0.1f, 0));

            // Lampu 1
            textlampu1 = textjudullampu.CreateChild("textlampu1");
            //textNoise = suhuNode.CreateChild();
            var tekslampu1 = textlampu1.CreateComponent<Text3D>();
            tekslampu1.HorizontalAlignment = HorizontalAlignment.Center;
            tekslampu1.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekslampu1.Text = "Lampu 1";
            tekslampu1.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            tekslampu1.SetColor(Color.White);
            textlampu1.Translate(new Vector3(-0.8f, -0.2f, 0));

            // Slider Lampu 1
            lampu1plus = textjudullampu.CreateChild("lampu1plus");
            lampu1plus.SetScale(0.5F);
            lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
            var plus5 = lampu1plus.CreateComponent<StaticModel>();
            plus5.Model = ResourceCache.GetModel("Models/KnobPlus.mdl");
            plus5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));

            lampu1min = textjudullampu.CreateChild("lampu1min");
            lampu1min.SetScale(0.5F);
            lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            var min5 = lampu1min.CreateComponent<StaticModel>();
            min5.Model = ResourceCache.GetModel("Models/KnobMin.mdl");
            min5.SetMaterial(ResourceCache.GetMaterial("Materials/hum0.xml"));

            nilailampu1 = textjudullampu.CreateChild("nilailampu1");
            nilailampu1.SetScale(0.4F);
            nilailampu1.Position = new Vector3(-0.8f, -1.6f, 0);
            var vallampu1 = nilailampu1.CreateComponent<StaticModel>();
            vallampu1.Model = ResourceCache.GetModel("Models/Slider.mdl");
            vallampu1.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));


            // Lampu 2
            textlampu2 = textjudullampu.CreateChild("textlampu2");
            //textNoise = suhuNode.CreateChild();
            var tekslampu2 = textlampu2.CreateComponent<Text3D>();
            tekslampu2.HorizontalAlignment = HorizontalAlignment.Center;
            tekslampu2.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekslampu2.Text = "Lampu 2";
            tekslampu2.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            tekslampu2.SetColor(Color.White);
            textlampu2.Translate(new Vector3(-0.3f, -0.2f, 0));

            // Slider Lampu 2
            lampu2plus = textjudullampu.CreateChild("lampu2plus");
            lampu2plus.SetScale(0.5F);
            lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
            var plus6 = lampu2plus.CreateComponent<StaticModel>();
            plus6.Model = ResourceCache.GetModel("Models/KnobPlus.mdl");
            plus6.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));

            lampu2min = textjudullampu.CreateChild("lampu2min");
            lampu2min.SetScale(0.5F);
            lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            var min6 = lampu2min.CreateComponent<StaticModel>();
            min6.Model = ResourceCache.GetModel("Models/KnobMin.mdl");
            min6.SetMaterial(ResourceCache.GetMaterial("Materials/hum0.xml"));

            nilailampu2 = textjudullampu.CreateChild("nilailampu2");
            nilailampu2.SetScale(0.4F);
            nilailampu2.Position = new Vector3(-0.3f, -1.6f, 0);
            var vallampu2 = nilailampu2.CreateComponent<StaticModel>();
            vallampu2.Model = ResourceCache.GetModel("Models/Slider.mdl");
            vallampu2.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            // Lampu 3
            textlampu3 = textjudullampu.CreateChild("textlampu3");
            //textNoise = suhuNode.CreateChild();
            var tekslampu3 = textlampu3.CreateComponent<Text3D>();
            tekslampu3.HorizontalAlignment = HorizontalAlignment.Center;
            tekslampu3.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekslampu3.Text = "Lampu 3";
            tekslampu3.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            tekslampu3.SetColor(Color.White);
            textlampu3.Translate(new Vector3(0.2f, -0.2f, 0));

            // Slider Lampu 3
            lampu3plus = textjudullampu.CreateChild("lampu3plus");
            lampu3plus.SetScale(0.5F);
            lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
            var plus7 = lampu3plus.CreateComponent<StaticModel>();
            plus7.Model = ResourceCache.GetModel("Models/KnobPlus.mdl");
            plus7.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));

            lampu3min = textjudullampu.CreateChild("lampu3min");
            lampu3min.SetScale(0.5F);
            lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            var min7 = lampu3min.CreateComponent<StaticModel>();
            min7.Model = ResourceCache.GetModel("Models/KnobMin.mdl");
            min7.SetMaterial(ResourceCache.GetMaterial("Materials/hum0.xml"));

            nilailampu3 = textjudullampu.CreateChild("nilailampu3");
            nilailampu3.SetScale(0.4F);
            nilailampu3.Position = new Vector3(0.2f, -1.6f, 0);
            var vallampu3 = nilailampu3.CreateComponent<StaticModel>();
            vallampu3.Model = ResourceCache.GetModel("Models/Slider.mdl");
            vallampu3.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            // Lampu 4
            textlampu4 = textjudullampu.CreateChild("textlampu4");
            //textNoise = suhuNode.CreateChild();
            var tekslampu4 = textlampu4.CreateComponent<Text3D>();
            tekslampu4.HorizontalAlignment = HorizontalAlignment.Center;
            tekslampu4.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekslampu4.Text = "Lampu 4";
            tekslampu4.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            tekslampu4.SetColor(Color.White);
            textlampu4.Translate(new Vector3(0.7f, -0.2f, 0));

            // Slider Lampu 4
            lampu4plus = textjudullampu.CreateChild("lampu4plus");
            lampu4plus.SetScale(0.5F);
            lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
            var plus8 = lampu4plus.CreateComponent<StaticModel>();
            plus8.Model = ResourceCache.GetModel("Models/KnobPlus.mdl");
            plus8.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));

            lampu4min = textjudullampu.CreateChild("lampu4min");
            lampu4min.SetScale(0.5F);
            lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            var min8 = lampu4min.CreateComponent<StaticModel>();
            min8.Model = ResourceCache.GetModel("Models/KnobMin.mdl");
            min8.SetMaterial(ResourceCache.GetMaterial("Materials/hum0.xml"));

            nilailampu4 = textjudullampu.CreateChild("nilailampu4");
            nilailampu4.SetScale(0.4F);
            nilailampu4.Position = new Vector3(0.7f, -1.6f, 0);
            var vallampu4 = nilailampu4.CreateComponent<StaticModel>();
            vallampu4.Model = ResourceCache.GetModel("Models/Slider.mdl");
            vallampu4.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            // Kontrol Posisi Node Lampu
            uplampu = textjudullampu.CreateChild("uplampu");
            uplampu.SetScale(0.3F);
            uplampu.Position = new Vector3(0, 0, 0);
            uplampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up5 = uplampu.CreateComponent<StaticModel>();
            up5.Model = ResourceCache.GetModel("Models/panah.mdl");
            up5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            rightlampu = textjudullampu.CreateChild("rightlampu");
            rightlampu.SetScale(0.3F);
            rightlampu.Position = new Vector3(1.2f, -0.9f, 0);
            rightlampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right5 = rightlampu.CreateComponent<StaticModel>();
            right5.Model = ResourceCache.GetModel("Models/panah.mdl");
            right5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            downlampu = textjudullampu.CreateChild("downlampu");
            downlampu.SetScale(0.3F);
            downlampu.Position = new Vector3(0, -1.6f, 0);
            downlampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down5 = downlampu.CreateComponent<StaticModel>();
            down5.Model = ResourceCache.GetModel("Models/panah.mdl");
            down5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            leftlampu = textjudullampu.CreateChild("leftlampu");
            leftlampu.SetScale(0.3F);
            leftlampu.Position = new Vector3(-1.2f, -0.9f, 0);
            leftlampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left5 = leftlampu.CreateComponent<StaticModel>();
            left5.Model = ResourceCache.GetModel("Models/panah.mdl");
            left5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            frontlampu = textjudullampu.CreateChild("frontlampu");
            frontlampu.SetScale(0.3F);
            frontlampu.Position = new Vector3(-1.1f, -0.8f, 0);
            frontlampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front5 = frontlampu.CreateComponent<StaticModel>();
            front5.Model = ResourceCache.GetModel("Models/panah.mdl");
            front5.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backlampu = textjudullampu.CreateChild("backlampu");
            backlampu.SetScale(0.3F);
            backlampu.Position = new Vector3(-1.1f, -1, 0);
            backlampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back5 = backlampu.CreateComponent<StaticModel>();
            back5.Model = ResourceCache.GetModel("Models/panah.mdl");
            back5.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Lampu
            pluslampu = textjudullampu.CreateChild("pluslampu");
            pluslampu.SetScale(0.05F);
            pluslampu.Position = new Vector3(0.95f, -0.8f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus9 = pluslampu.CreateComponent<StaticModel>();
            plus9.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus9.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bpluslampu = textjudullampu.CreateChild("pluslampu");
            bpluslampu.SetScale(0.05F);
            bpluslampu.Position = new Vector3(0.95f, -0.8f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus9 = bpluslampu.CreateComponent<StaticModel>();
            bplus9.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus9.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minlampu = textjudullampu.CreateChild("minlampu");
            minlampu.SetScale(0.05F);
            minlampu.Position = new Vector3(0.95f, -1.1f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min9 = minlampu.CreateComponent<StaticModel>();
            min9.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min9.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminlampu = textjudullampu.CreateChild("minlampu");
            bminlampu.SetScale(0.05F);
            bminlampu.Position = new Vector3(0.95f, -1.1f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin9 = bminlampu.CreateComponent<StaticModel>();
            bmin9.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin9.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            // Kontrol Rotasi Node Lampu
            rotate_pos_lampu = textjudullampu.CreateChild("rotate_pos_lampu");
            rotate_pos_lampu.SetScale(0.25F);
            rotate_pos_lampu.Position = new Vector3(-0.8f, 0.1f, 0);
            //rotate_pos_lampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 0), 0);
            var rotasi_pos_lampu = rotate_pos_lampu.CreateComponent<StaticModel>();
            rotasi_pos_lampu.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_pos_lampu.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            rotate_neg_lampu = textjudullampu.CreateChild("rotate_neg_lampu");
            rotate_neg_lampu.SetScale(0.25F);
            rotate_neg_lampu.Position = new Vector3(0.8f, 0.1f, 0);
            rotate_neg_lampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 180);
            var rotasi_neg_lampu = rotate_neg_lampu.CreateComponent<StaticModel>();
            rotasi_neg_lampu.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_neg_lampu.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));
        }

        public void tampilanKontrolKipas()
        {
            textjudulkipas = Scene.CreateChild("Kontrol Kipas");
            textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
            textjudulkipas.SetScale(kipas.skala);
            var textkipas = textjudulkipas.CreateComponent<Text3D>();
            textkipas.HorizontalAlignment = HorizontalAlignment.Center;
            textkipas.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textkipas.Text = "Kontrol Kipas";
            textkipas.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textkipas.SetColor(Color.Blue);
            //textNoise.Translate(new Vector3(0, -0.1f, 0));

            // Kipas 1
            textkipas1 = textjudulkipas.CreateChild("textkipas1");
            //textNoise = suhuNode.CreateChild();
            var tekskipas1 = textkipas1.CreateComponent<Text3D>();
            tekskipas1.HorizontalAlignment = HorizontalAlignment.Center;
            tekskipas1.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekskipas1.Text = "Kipas 1";
            tekskipas1.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            tekskipas1.SetColor(Color.White);
            textkipas1.Translate(new Vector3(-0.8f, -0.2f, 0));

            // Button Kipas 1
            kipas1plus = textjudulkipas.CreateChild("kipas1plus");
            kipas1plus.SetScale(0.5F);
            kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
            var plus9 = kipas1plus.CreateComponent<StaticModel>();
            plus9.Model = ResourceCache.GetModel("Models/KnobPlus.mdl");
            plus9.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));

            kipas1min = textjudulkipas.CreateChild("kipas1min");
            kipas1min.SetScale(0.5F);
            kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            var min10 = kipas1min.CreateComponent<StaticModel>();
            min10.Model = ResourceCache.GetModel("Models/KnobMin.mdl");
            min10.SetMaterial(ResourceCache.GetMaterial("Materials/hum0.xml"));

            nilaikipas1 = textjudulkipas.CreateChild("nilaikipas1");
            nilaikipas1.SetScale(0.4F);
            nilaikipas1.Position = new Vector3(-0.8f, -1.6f, 0);
            var valkipas1 = nilaikipas1.CreateComponent<StaticModel>();
            valkipas1.Model = ResourceCache.GetModel("Models/Slider.mdl");
            valkipas1.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));


            // Kipas 2
            textkipas2 = textjudulkipas.CreateChild("textkipas2");
            //textNoise = suhuNode.CreateChild();
            var tekskipas2 = textkipas2.CreateComponent<Text3D>();
            tekskipas2.HorizontalAlignment = HorizontalAlignment.Center;
            tekskipas2.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekskipas2.Text = "Kipas 2";
            tekskipas2.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            tekskipas2.SetColor(Color.White);
            textkipas2.Translate(new Vector3(-0.3f, -0.2f, 0));

            // Slider Kipas 2
            kipas2plus = textjudulkipas.CreateChild("kipas2plus");
            kipas2plus.SetScale(0.5F);
            kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
            var plus11 = kipas2plus.CreateComponent<StaticModel>();
            plus11.Model = ResourceCache.GetModel("Models/KnobPlus.mdl");
            plus11.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));

            kipas2min = textjudulkipas.CreateChild("kipas2min");
            kipas2min.SetScale(0.5F);
            kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            var min11 = kipas2min.CreateComponent<StaticModel>();
            min11.Model = ResourceCache.GetModel("Models/KnobMin.mdl");
            min11.SetMaterial(ResourceCache.GetMaterial("Materials/hum0.xml"));

            nilaikipas2 = textjudulkipas.CreateChild("nilaikipas2");
            nilaikipas2.SetScale(0.4F);
            nilaikipas2.Position = new Vector3(-0.3f, -1.6f, 0);
            var valkipas2 = nilaikipas2.CreateComponent<StaticModel>();
            valkipas2.Model = ResourceCache.GetModel("Models/Slider.mdl");
            valkipas2.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            // Kontrol Posisi Node Kipas
            upkipas = textjudulkipas.CreateChild("upkipas");
            upkipas.SetScale(0.3F);
            upkipas.Position = new Vector3(0, 0, 0);
            upkipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up6 = upkipas.CreateComponent<StaticModel>();
            up6.Model = ResourceCache.GetModel("Models/panah.mdl");
            up6.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            rightkipas = textjudulkipas.CreateChild("rightkipas");
            rightkipas.SetScale(0.3F);
            rightkipas.Position = new Vector3(1.2f, -0.9f, 0);
            rightkipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right6 = rightkipas.CreateComponent<StaticModel>();
            right6.Model = ResourceCache.GetModel("Models/panah.mdl");
            right6.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            downkipas = textjudulkipas.CreateChild("downkipas");
            downkipas.SetScale(0.3F);
            downkipas.Position = new Vector3(0, -1.6f, 0);
            downkipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down6 = downkipas.CreateComponent<StaticModel>();
            down6.Model = ResourceCache.GetModel("Models/panah.mdl");
            down6.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            leftkipas = textjudulkipas.CreateChild("leftkipas");
            leftkipas.SetScale(0.3F);
            leftkipas.Position = new Vector3(-1.2f, -0.9f, 0);
            leftkipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left6 = leftkipas.CreateComponent<StaticModel>();
            left6.Model = ResourceCache.GetModel("Models/panah.mdl");
            left6.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            frontkipas = textjudulkipas.CreateChild("frontkipas");
            frontkipas.SetScale(0.3F);
            frontkipas.Position = new Vector3(-1.1f, -0.8f, 0);
            frontkipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front6 = frontkipas.CreateComponent<StaticModel>();
            front6.Model = ResourceCache.GetModel("Models/panah.mdl");
            front6.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backkipas = textjudulkipas.CreateChild("backkipas");
            backkipas.SetScale(0.3F);
            backkipas.Position = new Vector3(-1.1f, -1, 0);
            backkipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back6 = backkipas.CreateComponent<StaticModel>();
            back6.Model = ResourceCache.GetModel("Models/panah.mdl");
            back6.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Kipas
            pluskipas = textjudulkipas.CreateChild("pluskipas");
            pluskipas.SetScale(0.05F);
            pluskipas.Position = new Vector3(0.95f, -0.8f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus12 = pluskipas.CreateComponent<StaticModel>();
            plus12.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus12.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bpluskipas = textjudulkipas.CreateChild("pluskipas");
            bpluskipas.SetScale(0.05F);
            bpluskipas.Position = new Vector3(0.95f, -0.8f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus12 = bpluskipas.CreateComponent<StaticModel>();
            bplus12.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus12.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minkipas = textjudulkipas.CreateChild("minkipas");
            minkipas.SetScale(0.05F);
            minkipas.Position = new Vector3(0.95f, -1.1f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min12 = minkipas.CreateComponent<StaticModel>();
            min12.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min12.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminkipas = textjudulkipas.CreateChild("minkipas");
            bminkipas.SetScale(0.05F);
            bminkipas.Position = new Vector3(0.95f, -1.1f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin12 = bminkipas.CreateComponent<StaticModel>();
            bmin12.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin12.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            // Kontrol Rotasi Node Kipas
            rotate_pos_kipas = textjudulkipas.CreateChild("rotate_pos_kipas");
            rotate_pos_kipas.SetScale(0.25F);
            rotate_pos_kipas.Position = new Vector3(-0.8f, 0.1f, 0);
            //rotate_pos_kipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 0), 0);
            var rotasi_pos_kipas = rotate_pos_kipas.CreateComponent<StaticModel>();
            rotasi_pos_kipas.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_pos_kipas.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));

            rotate_neg_kipas = textjudulkipas.CreateChild("rotate_neg_kipas");
            rotate_neg_kipas.SetScale(0.25F);
            rotate_neg_kipas.Position = new Vector3(0.8f, 0.1f, 0);
            rotate_neg_kipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 180);
            var rotasi_neg_kipas = rotate_neg_kipas.CreateComponent<StaticModel>();
            rotasi_neg_kipas.Model = ResourceCache.GetModel("Models/Rotate.mdl");
            rotasi_neg_kipas.SetMaterial(ResourceCache.GetMaterial("Materials/outtemp.xml"));
        }

        // Fungsi Menerima Model dan Nomor Kelas
        public void tampilanPilihanKelas()
        {
            PilihanModeKelas = Scene.CreateChild("Pilihan Kelas");
            PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
            PilihanModeKelas.SetScale(kelas.skala);
            var textkelas = PilihanModeKelas.CreateComponent<Text3D>();
            textkelas.HorizontalAlignment = HorizontalAlignment.Center;
            textkelas.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textkelas.Text = "Mode dan Pilihan Kelas";
            textkelas.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textkelas.SetColor(Color.Blue);
            //textNoise.Translate(new Vector3(0, -0.1f, 0));

            // Pilihan 1
            Pilihan1 = PilihanModeKelas.CreateChild("Pilihan1");
            //textNoise = suhuNode.CreateChild();
            var tekspilihan1 = Pilihan1.CreateComponent<Text3D>();
            tekspilihan1.HorizontalAlignment = HorizontalAlignment.Center;
            tekspilihan1.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekspilihan1.Text = "Normal";
            tekspilihan1.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
            tekspilihan1.SetColor(Color.White);
            Pilihan1.Translate(new Vector3(-0.8f, -0.2f, 0));

            // Pilihan 2
            Pilihan2 = PilihanModeKelas.CreateChild("Pilihan2");
            //textNoise = suhuNode.CreateChild();
            var tekspilihan2 = Pilihan2.CreateComponent<Text3D>();
            tekspilihan2.HorizontalAlignment = HorizontalAlignment.Center;
            tekspilihan2.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekspilihan2.Text = "Presentasi";
            tekspilihan2.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
            tekspilihan2.SetColor(Color.White);
            Pilihan2.Translate(new Vector3(0, -0.2f, 0));

            // Pilihan 3
            Pilihan3 = PilihanModeKelas.CreateChild("Pilihan3");
            //textNoise = suhuNode.CreateChild();
            var tekspilihan3 = Pilihan3.CreateComponent<Text3D>();
            tekspilihan3.HorizontalAlignment = HorizontalAlignment.Center;
            tekspilihan3.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            tekspilihan3.Text = "Diskusi";
            tekspilihan3.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
            tekspilihan3.SetColor(Color.White);
            Pilihan3.Translate(new Vector3(0.7f, -0.2f, 0));

            // Kontrol Posisi Node Pilihan
            upkelas = PilihanModeKelas.CreateChild("upkelas");
            upkelas.SetScale(0.3F);
            upkelas.Position = new Vector3(0, 0, 0);
            upkelas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up7 = upkelas.CreateComponent<StaticModel>();
            up7.Model = ResourceCache.GetModel("Models/panah.mdl");
            up7.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            rightkelas = PilihanModeKelas.CreateChild("rightkelas");
            rightkelas.SetScale(0.3F);
            rightkelas.Position = new Vector3(1.6f, 0, 0);
            rightkelas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right7 = rightkelas.CreateComponent<StaticModel>();
            right7.Model = ResourceCache.GetModel("Models/panah.mdl");
            right7.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            downkelas = PilihanModeKelas.CreateChild("downkelas");
            downkelas.SetScale(0.3F);
            downkelas.Position = new Vector3(0, -0.4f, 0);
            downkelas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down7 = downkelas.CreateComponent<StaticModel>();
            down7.Model = ResourceCache.GetModel("Models/panah.mdl");
            down7.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            leftkelas = PilihanModeKelas.CreateChild("leftkelas");
            leftkelas.SetScale(0.3F);
            leftkelas.Position = new Vector3(-1.6f, -0.1f, 0);
            leftkelas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left7 = leftkelas.CreateComponent<StaticModel>();
            left7.Model = ResourceCache.GetModel("Models/panah.mdl");
            left7.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            frontkelas = PilihanModeKelas.CreateChild("frontkelas");
            frontkelas.SetScale(0.3F);
            frontkelas.Position = new Vector3(-1.5f, 0, 0);
            frontkelas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front7 = frontkelas.CreateComponent<StaticModel>();
            front7.Model = ResourceCache.GetModel("Models/panah.mdl");
            front7.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backkelas = PilihanModeKelas.CreateChild("backkelas");
            backkelas.SetScale(0.3F);
            backkelas.Position = new Vector3(-1.5f, -0.2f, 0);
            backkelas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back7 = backkelas.CreateComponent<StaticModel>();
            back7.Model = ResourceCache.GetModel("Models/panah.mdl");
            back7.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Noise
            pluskelas = PilihanModeKelas.CreateChild("pluskelas");
            pluskelas.SetScale(0.05F);
            pluskelas.Position = new Vector3(1.5f, 0, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus15 = pluskelas.CreateComponent<StaticModel>();
            plus15.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus15.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bpluskelas = PilihanModeKelas.CreateChild("pluskelas");
            bpluskelas.SetScale(0.05F);
            bpluskelas.Position = new Vector3(1.5f, 0, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus15 = bpluskelas.CreateComponent<StaticModel>();
            bplus15.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus15.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minkelas = PilihanModeKelas.CreateChild("minkelas");
            minkelas.SetScale(0.05F);
            minkelas.Position = new Vector3(1.5f, -0.2f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min15 = minkelas.CreateComponent<StaticModel>();
            min15.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min15.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminkelas = PilihanModeKelas.CreateChild("minkelas");
            bminkelas.SetScale(0.05F);
            bminkelas.Position = new Vector3(1.5f, -0.2f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin15 = bminkelas.CreateComponent<StaticModel>();
            bmin15.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin15.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));
        }

        // Fungsi Untuk Tiap Gambar Suhu
        public void GambarSuhu(decimal a)
        {
            if (a < 20)
            {
                var suhu = suhuNode.GetComponent<StaticModel>();
                suhu.Model = ResourceCache.GetModel("Models/intemp0.mdl");
                suhu.SetMaterial(ResourceCache.GetMaterial("Materials/intemp0.xml"));

            }
            else if (a < 23)
            {
                var suhu = suhuNode.GetComponent<StaticModel>();
                suhu.Model = ResourceCache.GetModel("Models/intemp1.mdl");
                suhu.SetMaterial(ResourceCache.GetMaterial("Materials/intemp1.xml")); ;
            }
            else if (a <= 26)
            {
                var suhu = suhuNode.GetComponent<StaticModel>();
                suhu.Model = ResourceCache.GetModel("Models/intemp2.mdl");
                suhu.SetMaterial(ResourceCache.GetMaterial("Materials/intemp2.xml"));
            }
            else if (a <= 28)
            {
                var suhu = suhuNode.GetComponent<StaticModel>();
                suhu.Model = ResourceCache.GetModel("Models/intemp3.mdl");
                suhu.SetMaterial(ResourceCache.GetMaterial("Materials/intemp3.xml"));
            }
            else
            {
                var suhu = suhuNode.GetComponent<StaticModel>();
                suhu.Model = ResourceCache.GetModel("Models/intemp4.mdl");
                suhu.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));
            }
        }

        public void GambarKelembapan(decimal a)
        {
            if (a <= 30)
            {
                var lembap = lembapNode.GetComponent<StaticModel>();
                lembap.Model = ResourceCache.GetModel("Models/hum0.mdl");
                lembap.SetMaterial(ResourceCache.GetMaterial("Materials/hum0.xml"));
            }
            else if (a <= 40)
            {
                var lembap = lembapNode.GetComponent<StaticModel>();
                lembap.Model = ResourceCache.GetModel("Models/hum1.mdl");
                lembap.SetMaterial(ResourceCache.GetMaterial("Materials/hum1.xml"));
            }
            else if (a <= 60)
            {
                var lembap = lembapNode.GetComponent<StaticModel>();
                lembap.Model = ResourceCache.GetModel("Models/hum2.mdl");
                lembap.SetMaterial(ResourceCache.GetMaterial("Materials/hum2.xml"));
            }
            else if (a <= 80)
            {
                var lembap = lembapNode.GetComponent<StaticModel>();
                lembap.Model = ResourceCache.GetModel("Models/hum3.mdl");
                lembap.SetMaterial(ResourceCache.GetMaterial("Materials/hum3.xml"));
            }
            else
            {
                var lembap = lembapNode.GetComponent<StaticModel>();
                lembap.Model = ResourceCache.GetModel("Models/hum4.mdl");
                lembap.SetMaterial(ResourceCache.GetMaterial("Materials/hum3.xml"));
            }
        }

        public void GambarIntensitas(int a)
        {
            if (nmode == 0)
            {
                if (a <= 250)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/bohlam0.xml"));
                }
                else if (a <= 300)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light1.xml"));
                }
                else if (a <= 500)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light2.xml"));
                }
                else if (a <= 750)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam3.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light3.xml"));
                }
                else
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam4.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light4.xml"));
                }
            }
            else if (nmode == 1)
            {
                if (a <= 10)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/bohlam0.xml"));
                }
                else if (a <= 50)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light1.xml"));
                }
                else if (a <= 100)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light2.xml"));
                }
                else if (a <= 150)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam3.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light3.xml"));
                }
                else
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam4.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light4.xml"));
                }
            }
            else
            {
                if (a <= 250)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/bohlam0.xml"));
                }
                else if (a <= 300)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light1.xml"));
                }
                else if (a <= 500)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam012.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light2.xml"));
                }
                else if (a <= 750)
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam3.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light3.xml"));
                }
                else
                {
                    var cahaya = cahayaNode.GetComponent<StaticModel>();
                    cahaya.Model = ResourceCache.GetModel("Models/bohlam4.mdl");
                    cahaya.SetMaterial(ResourceCache.GetMaterial("Materials/light4.xml"));
                }
            }
        }

        public void GambarKebisingan(int a)
        {
            if (nmode == 0)
            {
                if (a <= 40)
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise1.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise1.xml"));
                }
                else if (a <= 50)
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise2.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise2.xml"));
                }
                else
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise3.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise3.xml"));
                }
            }
            else if (nmode == 1)
            {
                if (a <= 50)
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise1.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise1.xml"));
                }
                else if (a <= 60)
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise2.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise2.xml"));
                }
                else
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise3.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise3.xml"));
                }
            }
            else
            {
                if (a <= 60)
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise1.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise1.xml"));
                }
                else if (a <= 70)
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise2.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise2.xml"));
                }
                else
                {
                    var bising = bisingNode.GetComponent<StaticModel>();
                    bising.Model = ResourceCache.GetModel("Models/noise3.mdl");
                    bising.SetMaterial(ResourceCache.GetMaterial("Materials/noise3.xml"));
                }
            }
        }


        //Fungsi untuk Menampilkan Teks
        public void NilaiSuhu(decimal a)
        {
            var text3D1 = textTemp.GetComponent<Text3D>();
            text3D1.HorizontalAlignment = HorizontalAlignment.Center;
            text3D1.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D1.Text = "Suhu Ruang :" + a + " Derajat Celsius";
            text3D1.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            if (a <= 20)
            {
                text3D1.SetColor(Color.Red);

            }
            else if (a <= 23)
            {
                text3D1.SetColor(Color.Red);
            }
            else if (a <= 26)
            {
                text3D1.SetColor(Color.White);
            }
            else if (a <= 28)
            {
                text3D1.SetColor(Color.Red);
            }
            else
            {
                text3D1.SetColor(Color.Red);
            }
            //textTemp.Translate(new Vector3(0, 0, 1));
        }

        void NilaiKelembapan(decimal a)
        {
            //textHum = suhuNode.CreateChild();
            var text3D2 = textHum.GetComponent<Text3D>();
            text3D2.HorizontalAlignment = HorizontalAlignment.Center;
            text3D2.VerticalAlignment = VerticalAlignment.Top;
            //  text3D2.ViewMask = 0x80000000; //hide from raycasts
            text3D2.Text = "Kelembapan Ruang: " + a + " %";
            text3D2.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            if (a <= 30)
            {
                text3D2.SetColor(Color.Red);
            }
            else if (a <= 40)
            {
                text3D2.SetColor(Color.Red);
            }
            else if (a <= 60)
            {
                text3D2.SetColor(Color.White);
            }
            else if (a <= 80)
            {
                text3D2.SetColor(Color.Red);
            }
            else
            {
                text3D2.SetColor(Color.Red);
            }
            //textHum.Translate(new Vector3(0, 0, 1));
            //textHum.Translate(new Vector3(0, -3, 1));
        }

        void NilaiIntensitas(int a)
        {
            //textIntens = suhuNode.CreateChild();
            var text3D3 = textIntens.GetComponent<Text3D>();
            text3D3.HorizontalAlignment = HorizontalAlignment.Center;
            text3D3.VerticalAlignment = VerticalAlignment.Top;
            //  text3D3.ViewMask = 0x80000000; //hide from raycasts
            text3D3.Text = "Intensitas Cahaya Ruang:" + a + " Lux";
            text3D3.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            if (nmode == 0)
            {
                if (a <= 250)
                {
                    text3D3.SetColor(Color.Red);
                }
                else if (a <= 300)
                {
                    text3D3.SetColor(Color.Red);
                }
                else if (a <= 500)
                {
                    text3D3.SetColor(Color.White);
                }
                else if (a <= 750)
                {
                    text3D3.SetColor(Color.Red);
                }
                else
                {
                    text3D3.SetColor(Color.Red);
                }
            }
            else if (nmode == 1)
            {
                if (a <= 250)
                {
                    text3D3.SetColor(Color.Red);
                }
                else if (a <= 300)
                {
                    text3D3.SetColor(Color.Red);
                }
                else if (a <= 500)
                {
                    text3D3.SetColor(Color.White);
                }
                else if (a <= 750)
                {
                    text3D3.SetColor(Color.Red);
                }
                else
                {
                    text3D3.SetColor(Color.Red);
                }
            }
            else
            {
                if (a <= 10)
                {
                    text3D3.SetColor(Color.Red);
                }
                else if (a <= 50)
                {
                    text3D3.SetColor(Color.Red);
                }
                else if (a <= 100)
                {
                    text3D3.SetColor(Color.White);
                }
                else if (a <= 250)
                {
                    text3D3.SetColor(Color.Red);
                }
                else
                {
                    text3D3.SetColor(Color.Red);
                }
            }
            // textIntens.Translate(new Vector3(0, 0, 1));
            //textIntens.Translate(new Vector3(8, 0, 1));
        }

        void NilaiKebisingan(int a)
        {
            //textNoise = suhuNode.CreateChild();
            var text3D4 = textNoise.GetComponent<Text3D>();
            text3D4.HorizontalAlignment = HorizontalAlignment.Center;
            text3D4.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            text3D4.Text = "Kebisingan Ruang:" + a + " Desibel";
            text3D4.SetFont(CoreAssets.Fonts.AnonymousPro, 18);
            if (nmode == 0)
            {
                if (a <= 40)
                {
                    text3D4.SetColor(Color.White);
                }
                else if (a <= 50)
                {
                    text3D4.SetColor(Color.Red);
                }
                else
                {
                    text3D4.SetColor(Color.Red);
                }
            }
            else if (nmode == 1)
            {
                if (a <= 50)
                {
                    text3D4.SetColor(Color.White);
                }
                else if (a <= 60)
                {
                    text3D4.SetColor(Color.Red);
                }
                else
                {
                    text3D4.SetColor(Color.Red);
                }
            }
            else
            {
                if (a <= 60)
                {
                    text3D4.SetColor(Color.White);
                }
                else if (a <= 70)
                {
                    text3D4.SetColor(Color.Red);
                }
                else
                {
                    text3D4.SetColor(Color.Red);
                }
            }

        }

        // Fungsi Memasukan Jarak Sensor
        public void tampilanKontrolJarak()
        {
            textjuduljarak = Scene.CreateChild("Kontrol Jarak");
            textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
            textjuduljarak.SetScale(jarak.skala);
            var textjarak = textjuduljarak.CreateComponent<Text3D>();
            textjarak.HorizontalAlignment = HorizontalAlignment.Center;
            textjarak.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textjarak.Text = "Jarak Sensor (cm)";
            textjarak.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textjarak.SetColor(Color.Blue);
            //textNoise.Translate(new Vector3(0, -0.1f, 0));

            // Jarak 1
            textjarak1 = textjuduljarak.CreateChild("textjarak1");
            //textNoise = suhuNode.CreateChild();
            var teksjarak1 = textjarak1.CreateComponent<Text3D>();
            teksjarak1.HorizontalAlignment = HorizontalAlignment.Center;
            teksjarak1.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            teksjarak1.Text = "Digit 1";
            teksjarak1.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            teksjarak1.SetColor(Color.White);
            textjarak1.Translate(new Vector3(-0.6f, -0.2f, 0));

            // Button Jarak 1
            jarak1plus = textjuduljarak.CreateChild("jarak1plus");
            jarak1plus.SetScale(0.05F);
            jarak1plus.Position = new Vector3(-0.6f, -0.5f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus5 = jarak1plus.CreateComponent<StaticModel>();
            plus5.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus5.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            jarak1min = textjuduljarak.CreateChild("jarak1min");
            jarak1min.SetScale(0.05F);
            jarak1min.Position = new Vector3(-0.6f, -0.9f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min5 = jarak1min.CreateComponent<StaticModel>();
            min5.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min5.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            nilaijarak1 = textjuduljarak.CreateChild("nilaijarak1");
            //textNoise = suhuNode.CreateChild();
            var valjarak1 = nilaijarak1.CreateComponent<Text3D>();
            valjarak1.HorizontalAlignment = HorizontalAlignment.Center;
            valjarak1.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            valjarak1.Text = "" + njarak1;
            valjarak1.SetFont(CoreAssets.Fonts.AnonymousPro, 22);
            valjarak1.SetColor(Color.White);
            nilaijarak1.Translate(new Vector3(-0.6f, -0.5f, 0));


            // Jarak 2
            textjarak2 = textjuduljarak.CreateChild("textjarak2");
            //textNoise = suhuNode.CreateChild();
            var teksjarak2 = textjarak2.CreateComponent<Text3D>();
            teksjarak2.HorizontalAlignment = HorizontalAlignment.Center;
            teksjarak2.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            teksjarak2.Text = "Digit 2";
            teksjarak2.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            teksjarak2.SetColor(Color.White);
            textjarak2.Translate(new Vector3(-0.1f, -0.2f, 0));

            // Button Jarak 2
            jarak2plus = textjuduljarak.CreateChild("jarak2plus");
            jarak2plus.SetScale(0.05F);
            jarak2plus.Position = new Vector3(-0.1f, -0.5f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus6 = jarak2plus.CreateComponent<StaticModel>();
            plus6.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus6.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            jarak2min = textjuduljarak.CreateChild("jarak2min");
            jarak2min.SetScale(0.05F);
            jarak2min.Position = new Vector3(-0.1f, -0.9f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min6 = jarak2min.CreateComponent<StaticModel>();
            min6.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min6.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            nilaijarak2 = textjuduljarak.CreateChild("nilaijarak2");
            //textNoise = suhuNode.CreateChild();
            var valjarak2 = nilaijarak2.CreateComponent<Text3D>();
            valjarak2.HorizontalAlignment = HorizontalAlignment.Center;
            valjarak2.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            valjarak2.Text = "" + njarak2;
            valjarak2.SetFont(CoreAssets.Fonts.AnonymousPro, 22);
            valjarak2.SetColor(Color.White);
            nilaijarak2.Translate(new Vector3(-0.1f, -0.5f, 0));

            // Jarak 3
            textjarak3 = textjuduljarak.CreateChild("textjarak3");
            //textNoise = suhuNode.CreateChild();
            var teksjarak3 = textjarak3.CreateComponent<Text3D>();
            teksjarak3.HorizontalAlignment = HorizontalAlignment.Center;
            teksjarak3.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            teksjarak3.Text = "Digit 3";
            teksjarak3.SetFont(CoreAssets.Fonts.AnonymousPro, 8);
            teksjarak3.SetColor(Color.White);
            textjarak3.Translate(new Vector3(0.4f, -0.2f, 0));

            // Button Jarak 3
            jarak3plus = textjuduljarak.CreateChild("jarak3plus");
            jarak3plus.SetScale(0.05F);
            jarak3plus.Position = new Vector3(0.4f, -0.5f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus7 = jarak3plus.CreateComponent<StaticModel>();
            plus7.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus7.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            jarak3min = textjuduljarak.CreateChild("jarak3min");
            jarak3min.SetScale(0.05F);
            jarak3min.Position = new Vector3(0.4f, -0.9f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min7 = jarak3min.CreateComponent<StaticModel>();
            min7.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min7.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            nilaijarak3 = textjuduljarak.CreateChild("nilaijarak3");
            //textNoise = suhuNode.CreateChild();
            var valjarak3 = nilaijarak3.CreateComponent<Text3D>();
            valjarak3.HorizontalAlignment = HorizontalAlignment.Center;
            valjarak3.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            valjarak3.Text = "" + njarak3;
            valjarak3.SetFont(CoreAssets.Fonts.AnonymousPro, 22);
            valjarak3.SetColor(Color.White);
            nilaijarak3.Translate(new Vector3(0.4f, -0.5f, 0));

            // Kontrol Posisi Node Jarak
            upjarak = textjuduljarak.CreateChild("upjarak");
            upjarak.SetScale(0.3F);
            upjarak.Position = new Vector3(0, 0, 0);
            upjarak.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var up5 = upjarak.CreateComponent<StaticModel>();
            up5.Model = ResourceCache.GetModel("Models/panah.mdl");
            up5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            rightjarak = textjuduljarak.CreateChild("rightjarak");
            rightjarak.SetScale(0.3F);
            rightjarak.Position = new Vector3(1.2f, -0.6f, 0);
            rightjarak.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 270);
            var right5 = rightjarak.CreateComponent<StaticModel>();
            right5.Model = ResourceCache.GetModel("Models/panah.mdl");
            right5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            downjarak = textjuduljarak.CreateChild("downjarak");
            downjarak.SetScale(0.3F);
            downjarak.Position = new Vector3(0, -0.9f, 0);
            downjarak.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var down5 = downjarak.CreateComponent<StaticModel>();
            down5.Model = ResourceCache.GetModel("Models/panah.mdl");
            down5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            leftjarak = textjuduljarak.CreateChild("leftjarak");
            leftjarak.SetScale(0.3F);
            leftjarak.Position = new Vector3(-1.2f, -0.6f, 0);
            leftjarak.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 90);
            var left5 = leftjarak.CreateComponent<StaticModel>();
            left5.Model = ResourceCache.GetModel("Models/panah.mdl");
            left5.SetMaterial(ResourceCache.GetMaterial("Materials/intemp4.xml"));

            frontjarak = textjuduljarak.CreateChild("frontjarak");
            frontjarak.SetScale(0.3F);
            frontjarak.Position = new Vector3(-1.1f, -0.5f, 0);
            frontjarak.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var front5 = frontjarak.CreateComponent<StaticModel>();
            front5.Model = ResourceCache.GetModel("Models/panah.mdl");
            front5.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            backjarak = textjuduljarak.CreateChild("backjarak");
            backjarak.SetScale(0.3F);
            backjarak.Position = new Vector3(-1.1f, -0.7f, 0);
            backjarak.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 180);
            var back5 = backjarak.CreateComponent<StaticModel>();
            back5.Model = ResourceCache.GetModel("Models/panah.mdl");
            back5.SetMaterial(ResourceCache.GetMaterial("Materials/panah.xml"));

            // Kontrol Skala Node Noise
            plusjarak = textjuduljarak.CreateChild("plusjarak");
            plusjarak.SetScale(0.05F);
            plusjarak.Position = new Vector3(0.95f, -0.5f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var plus9 = plusjarak.CreateComponent<StaticModel>();
            plus9.Model = ResourceCache.GetModel("Models/Plus.mdl");
            plus9.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bplusjarak = textjuduljarak.CreateChild("plusjarak");
            bplusjarak.SetScale(0.05F);
            bplusjarak.Position = new Vector3(0.95f, -0.5f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bplus9 = bplusjarak.CreateComponent<StaticModel>();
            bplus9.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bplus9.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));

            minjarak = textjuduljarak.CreateChild("minjarak");
            minjarak.SetScale(0.05F);
            minjarak.Position = new Vector3(0.95f, -0.8f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var min9 = minjarak.CreateComponent<StaticModel>();
            min9.Model = ResourceCache.GetModel("Models/Minus.mdl");
            min9.SetMaterial(ResourceCache.GetMaterial("Materials/Plus.xml"));

            bminjarak = textjuduljarak.CreateChild("minjarak");
            bminjarak.SetScale(0.05F);
            bminjarak.Position = new Vector3(0.95f, -0.8f, 0);
            //upsuhu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
            var bmin9 = bminjarak.CreateComponent<StaticModel>();
            bmin9.Model = ResourceCache.GetModel("Models/BackPlus.mdl");
            bmin9.SetMaterial(ResourceCache.GetMaterial("Materials/BackPlus.xml"));
        }

        public void NilaiJarak(int a, int b, int c)
        {
            var valjarak1 = nilaijarak1.GetComponent<Text3D>();
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            valjarak1.Text = "" + njarak1;
            //valjarak1.SetFont(CoreAssets.Fonts.AnonymousPro, 22);
            valjarak1.SetColor(Color.White);

            var valjarak2 = nilaijarak2.GetComponent<Text3D>();
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            valjarak2.Text = "" + njarak2;
            //valjarak1.SetFont(CoreAssets.Fonts.AnonymousPro, 22);
            valjarak2.SetColor(Color.White);

            var valjarak3 = nilaijarak3.GetComponent<Text3D>();
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            valjarak3.Text = "" + njarak3;
            //valjarak1.SetFont(CoreAssets.Fonts.AnonymousPro, 22);
            valjarak3.SetColor(Color.White);
        }

        public void ModeKelas(int a)
        {
            if (a == 0)
            {
                var tekspilihan1 = Pilihan1.GetComponent<Text3D>();
                tekspilihan1.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan1.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan1.Text = "Normal";
                tekspilihan1.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan1.SetColor(Color.Red);

                var tekspilihan2 = Pilihan2.GetComponent<Text3D>();
                tekspilihan2.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan2.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan2.Text = "Presentasi";
                tekspilihan2.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan2.SetColor(Color.White);

                var tekspilihan3 = Pilihan3.GetComponent<Text3D>();
                tekspilihan3.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan3.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan3.Text = "Diskusi";
                tekspilihan3.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan3.SetColor(Color.White);

                nmode = 0;
            }
            else if (a == 1)
            {
                var tekspilihan1 = Pilihan1.GetComponent<Text3D>();
                tekspilihan1.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan1.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan1.Text = "Normal";
                tekspilihan1.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan1.SetColor(Color.White);

                var tekspilihan2 = Pilihan2.GetComponent<Text3D>();
                tekspilihan2.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan2.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan2.Text = "Presentasi";
                tekspilihan2.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan2.SetColor(Color.Red);

                var tekspilihan3 = Pilihan3.GetComponent<Text3D>();
                tekspilihan3.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan3.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan3.Text = "Diskusi";
                tekspilihan3.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan3.SetColor(Color.White);

                nmode = 1;
            }
            else
            {
                var tekspilihan1 = Pilihan1.GetComponent<Text3D>();
                tekspilihan1.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan1.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan1.Text = "Normal";
                tekspilihan1.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan1.SetColor(Color.White);

                var tekspilihan2 = Pilihan2.GetComponent<Text3D>();
                tekspilihan2.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan2.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan2.Text = "Presentasi";
                tekspilihan2.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan2.SetColor(Color.White);

                var tekspilihan3 = Pilihan3.GetComponent<Text3D>();
                tekspilihan3.HorizontalAlignment = HorizontalAlignment.Center;
                tekspilihan3.VerticalAlignment = VerticalAlignment.Top;
                //  text3D4.ViewMask = 0x80000000; //hide from raycasts
                tekspilihan3.Text = "Diskusi";
                tekspilihan3.SetFont(CoreAssets.Fonts.AnonymousPro, 12);
                tekspilihan3.SetColor(Color.Red);

                nmode = 2;
            }

        }

        // Fungsi Menampilkan Pesan Error
        public async void PesanError_NilaiAktuatorDiatasBatas_Lampu()
        {
            var textlampu = textjudullampu.GetComponent<Text3D>();
            textlampu.HorizontalAlignment = HorizontalAlignment.Center;
            textlampu.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textlampu.Text = "Nilai Sudah Maksimal!";
            textlampu.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textlampu.SetColor(Color.Blue);

            await Delay(2f);
            textlampu.HorizontalAlignment = HorizontalAlignment.Center;
            textlampu.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textlampu.Text = "Kontrol Lampu";
            textlampu.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textlampu.SetColor(Color.Blue);


        }

        public async void PesanError_NilaiAktuatorDibawahBatas_Lampu()
        {
            var textlampu = textjudullampu.GetComponent<Text3D>();
            textlampu.HorizontalAlignment = HorizontalAlignment.Center;
            textlampu.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textlampu.Text = "Nilai Sudah Minimal!";
            textlampu.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textlampu.SetColor(Color.Blue);

            await Delay(2f);
            textlampu.HorizontalAlignment = HorizontalAlignment.Center;
            textlampu.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textlampu.Text = "Kontrol Lampu";
            textlampu.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textlampu.SetColor(Color.Blue);


        }

        public async void PesanError_NilaiAktuatorDiatasBatas_Kipas()
        {
            var textkipas = textjudulkipas.GetComponent<Text3D>();
            textkipas.HorizontalAlignment = HorizontalAlignment.Center;
            textkipas.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textkipas.Text = "Nilai Sudah Maksimal!";
            textkipas.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textkipas.SetColor(Color.Blue);

            await Delay(1f);
            textkipas.HorizontalAlignment = HorizontalAlignment.Center;
            textkipas.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textkipas.Text = "Kontrol Kipas";
            textkipas.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textkipas.SetColor(Color.Blue);


        }

        public async void PesanError_NilaiAktuatorDibawahBatas_Kipas()
        {
            var textkipas = textjudulkipas.GetComponent<Text3D>();
            textkipas.HorizontalAlignment = HorizontalAlignment.Center;
            textkipas.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textkipas.Text = "Nilai Sudah Minimal!";
            textkipas.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textkipas.SetColor(Color.Blue);

            await Delay(1f);
            textkipas.HorizontalAlignment = HorizontalAlignment.Center;
            textkipas.VerticalAlignment = VerticalAlignment.Top;
            //  text3D4.ViewMask = 0x80000000; //hide from raycasts
            textkipas.Text = "Kontrol Kipas";
            textkipas.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            textkipas.SetColor(Color.Blue);


        }

        // Fungsi Menampilkan Penanda Tengah
        void ShowCrosshair()
        //fungsi menampilkan crosshair
        {
            //buat crosshair
            crosshair = new Sprite(Context);
            crosshair.Texture = ResourceCache.GetTexture2D(@"Textures/reddot.png");
            crosshair.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            //crosshair.HotSpot = new IntVector2(0, 0);
            crosshair.Position = new IntVector2(-50, -50);
            crosshair.SetSize(100, 100);
            UI.Root.AddChild(crosshair);
        }

        // Fungsi Menerima Masukan
        public override void OnGestureTapped()
        {
            base.OnGestureTapped();
            Ray cameraRay = RightCamera.GetScreenRay(0.5f, 0.5f);

            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);

            // Bagian Pilihan Mode Teks dan Fitur
            if (result != null && result.Value.Node.Name == "Kotak3D")
            {
                if (KondisiRuang_3D == false)
                {
                    temp.skala = 0.04f; hum.skala = 0.04f; intens.skala = 0.04f; noise.skala = 0.04f; lampu.skala = 0.06f; kipas.skala = 0.06f; kelas.skala = 0.06f;
                    suhuNode.SetScale(temp.skala); lembapNode.SetScale(hum.skala); cahayaNode.SetScale(intens.skala); bisingNode.SetScale(noise.skala); textjudulkipas.SetScale(kipas.skala); textjudullampu.SetScale(lampu.skala); PilihanModeKelas.SetScale(kelas.skala);
                    KondisiRuang_3D = true;
                }
                else
                {
                    temp.skala = 0; hum.skala = 0; intens.skala = 0; noise.skala = 0; lampu.skala = 0; kipas.skala = 0; kelas.skala = 0;
                    suhuNode.SetScale(temp.skala); lembapNode.SetScale(hum.skala); cahayaNode.SetScale(intens.skala); bisingNode.SetScale(noise.skala); textjudulkipas.SetScale(kipas.skala); textjudullampu.SetScale(lampu.skala); PilihanModeKelas.SetScale(kelas.skala);
                    KondisiRuang_3D = false;
                }
            }
            else if (result != null && result.Value.Node.Name == "Kotak2D")
            {
                if (KondisiRuang_2D == false)
                {
                    if (rabbitmq.getData() != "")
                    {
                        ui.updateUI(dataJson.temperature, dataJson.humidity, dataJson.lightintensity, dataJson.soundlevel, nmode);
                        KondisiRuang_2D = true;
                    }
                    if (rabbitmq.getData_Voice() != "" && dataJson2.perintah != "\n" && dataJson2.perintah != "ian\n")
                    {
                        ui_Voice.updateUI(dataJson2.nama, dataJson2.perintah, dataJson2.jarak);
                        VoiceCommand_2D = true;
                    }
                    if (rabbitmq.getData_Face() != "")
                    {
                        ui_Face.updateUI(dataJson3.nama, dataJson3.ekspresi);
                        Face_2D = true;
                    }
                    if (rabbitmq.getData_Gesture() != "")
                    {
                        ui_Gesture.updateUI(dataJson4.cameraID, dataJson4.gesture, dataJson4.confidence);
                        Gesture_2D = true;
                    }
                    if (rabbitmq.getData_Gesture_Kinect() != "")
                    {
                        ui_Gesture.updateUI_Kinect(rabbitmq.getData_Gesture_Kinect());
                        Gesture_2D = true;
                    }
                }
                else
                {
                    KondisiRuang_2D = false;
                    VoiceCommand_2D = false;
                    Face_2D = false;
                    Gesture_2D = false;
                    ui.eraseUI();
                    ui_Voice.eraseUI();
                    ui_Face.eraseUI();
                    ui_Gesture.eraseUI();
                }
            }

            else if (result != null && result.Value.Node.Name == "Kotak4Sensor")
            {
                if (KondisiRuang_4Sensor == false)
                {
                    if (rabbitmq.getData() != "")
                    {
                        ui_4Sensor.updateUI(dataJson.temperature, dataJson.humidity, dataJson.lightintensity, dataJson.soundlevel, dataJson.sensor_id, nmode);
                        KondisiRuang_4Sensor = true;
                    }
                }
                else
                {
                    KondisiRuang_4Sensor = false;
                    ui_4Sensor.eraseUI();
                }
            }

            else if (result != null && result.Value.Node.Name == "KotakJarak")
            {
                if (JarakSensor == false)
                {
                    jarak.skala = 0.06f;
                    textjuduljarak.SetScale(jarak.skala);
                    JarakSensor = true;
                }
                else
                {
                    jarak.skala = 0;
                    textjuduljarak.SetScale(jarak.skala);
                    JarakSensor = false;
                }
            }

            // Bagian Kontrol Lampu

            else if (result != null && result.Value.Node.Name == "textlampu1")
            {
                slider1min.y = -1.57f;
                nlampu1 = 1000;
                slider1plus.y = -1.57f;
                kirimpesan(1);
                kirimpesan(1);
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            }

            else if (result != null && result.Value.Node.Name == "textlampu2")
            {
                slider2min.y = -1.57f;
                nlampu2 = 1000;
                slider2plus.y = -1.57f;
                kirimpesan(2);
                kirimpesan(2);
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            }

            else if (result != null && result.Value.Node.Name == "textlampu3")
            {
                slider3min.y = -1.57f;
                nlampu3 = 2000;
                slider3plus.y = -1.57f;
                kirimpesan(3);
                kirimpesan(3);
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            }
            else if (result != null && result.Value.Node.Name == "textlampu4")
            {
                slider4min.y = -1.57f;
                nlampu4 = 2000;
                slider4plus.y = -1.57f;
                kirimpesan(4);
                kirimpesan(4);
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu1plus")
            {
                if (slider1plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider1plus.y += 0.01f;
                    slider1min.y += 0.01f;
                    nlampu1 += 1;
                }
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu1min")
            {
                if (slider1min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider1plus.y -= 0.01f;
                    slider1min.y -= 0.01f;
                    nlampu1 -= 1;
                }
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu2plus")
            {
                if (slider2plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider2plus.y += 0.01f;
                    slider2min.y += 0.01f;
                    nlampu2 += 1;
                }
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu2min")
            {
                if (slider2min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider2plus.y -= 0.01f;
                    slider2min.y -= 0.01f;
                    nlampu2 -= 1;
                }
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu3plus")
            {
                if (slider3plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider3plus.y += 0.01f;
                    slider3min.y += 0.01f;
                    nlampu3 += 1;
                }
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu3min")
            {
                if (slider3min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider3plus.y -= 0.01f;
                    slider3min.y -= 0.01f;
                    nlampu3 -= 1;
                }
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu4plus")
            {
                if (slider4plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider4plus.y += 0.01f;
                    slider4min.y += 0.01f;
                    nlampu4 += 1;
                }
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu4min")
            {
                if (slider4min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider4plus.y -= 0.01f;
                    slider4min.y -= 0.01f;
                    nlampu4 -= 1;
                }
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas1plus")
            {
                if (slider5plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Kipas();
                }
                else
                {
                    slider5plus.y += 0.01f;
                    slider5min.y += 0.01f;
                    nkipas1 += 1;
                }
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas1min")
            {
                if (slider5min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Kipas();
                }
                else
                {
                    slider5plus.y -= 0.01f;
                    slider5min.y -= 0.01f;
                    nkipas1 -= 1;
                }
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas2plus")
            {
                if (slider6plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Kipas();
                }
                else
                {
                    slider6plus.y += 0.01f;
                    slider6min.y += 0.01f;
                    nkipas2 += 1;
                }
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas2min")
            {
                if (slider6min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Kipas();
                }
                else
                {
                    slider6plus.y -= 0.01f;
                    slider6min.y -= 0.01f;
                    nkipas2 -= 1;
                }
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }

            // Bagian Kontrol Aktuator Jarak
            else if (result != null && result.Value.Node.Name == "jarak1plus")
            {
                if (njarak1 >= 9)
                {
                }
                else
                {
                    njarak1 += 1;
                }
                NilaiJarak(njarak1, njarak2, njarak3);
            }
            else if (result != null && result.Value.Node.Name == "jarak1min")
            {
                if (njarak1 <= 0)
                {

                }
                else
                {
                    njarak1 -= 1;
                }
                NilaiJarak(njarak1, njarak2, njarak3);
            }
            else if (result != null && result.Value.Node.Name == "jarak2plus")
            {
                if (njarak2 >= 9)
                {
                }
                else
                {
                    njarak2 += 1;
                }
                NilaiJarak(njarak1, njarak2, njarak3);
            }
            else if (result != null && result.Value.Node.Name == "jarak2min")
            {
                if (njarak2 <= 0)
                {
                }
                else
                {
                    njarak2 -= 1;
                }
                NilaiJarak(njarak1, njarak2, njarak3);
            }
            else if (result != null && result.Value.Node.Name == "jarak3plus")
            {
                if (njarak3 >= 9)
                {
                }
                else
                {
                    njarak3 += 1;
                }
                NilaiJarak(njarak1, njarak2, njarak3);
            }
            else if (result != null && result.Value.Node.Name == "jarak3min")
            {
                if (njarak3 <= 0)
                {
                }
                else
                {
                    njarak3 -= 1;
                }
                NilaiJarak(njarak1, njarak2, njarak3);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Jarak")
            {

            }

            // Bagian Rotasi Node
            else if (result != null && result.Value.Node.Name == "rotate_pos_suhu")
            {
                temp.r += 10;
                suhuNode.Rotation = (Quaternion.FromAxisAngle(new Vector3(0, 1, 0), temp.r));
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rotate_neg_suhu")
            {
                temp.r -= 10;
                suhuNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), temp.r);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null && result.Value.Node.Name == "rotate_pos_hum")
            {
                hum.r += 10;
                lembapNode.Rotation = (Quaternion.FromAxisAngle(new Vector3(0, 1, 0), hum.r));
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rotate_neg_hum")
            {
                hum.r -= 10;
                lembapNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), hum.r);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null && result.Value.Node.Name == "rotate_pos_chy")
            {
                intens.r += 10;
                cahayaNode.Rotation = (Quaternion.FromAxisAngle(new Vector3(0, 1, 0), intens.r));
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rotate_neg_chy")
            {
                intens.r -= 10;
                cahayaNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), intens.r);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null && result.Value.Node.Name == "rotate_pos_noise")
            {
                noise.r += 10;
                bisingNode.Rotation = (Quaternion.FromAxisAngle(new Vector3(0, 1, 0), noise.r));
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rotate_neg_noise")
            {
                noise.r -= 10;
                bisingNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), noise.r);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null && result.Value.Node.Name == "rotate_pos_lampu")
            {
                lampu.r += 10;
                textjudullampu.Rotation = (Quaternion.FromAxisAngle(new Vector3(0, 1, 0), lampu.r));
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rotate_neg_lampu")
            {
                lampu.r -= 10;
                textjudullampu.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), lampu.r);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null && result.Value.Node.Name == "rotate_pos_kipas")
            {
                kipas.r += 10;
                textjudulkipas.Rotation = (Quaternion.FromAxisAngle(new Vector3(0, 1, 0), kipas.r));
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rotate_neg_kipas")
            {
                kipas.r -= 10;
                textjudulkipas.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), kipas.r);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Memilih Mode Kelas
            else if (result != null && result.Value.Node.Name == "Pilihan1")
            {
                ModeKelas(0);
                kirimpesan_modekelas();
            }
            else if (result != null && result.Value.Node.Name == "Pilihan2")
            {
                ModeKelas(1);
                kirimpesan_modekelas();
            }
            else if (result != null && result.Value.Node.Name == "Pilihan3")
            {
                ModeKelas(2);
                kirimpesan_modekelas();
            }

            // Bagian Mengirim Mode dan Nomor Kelas
            else if (result != null && result.Value.Node.Name == "Teks Digit Kelas")
            {
                kirimpesan_modekelas();
            }

            // Bagian Besar dan Kecil Node
            // Suhu
            else if (result != null && result.Value.Node.Name == "plussuhu")
            {
                temp.skala += 0.01f;
                suhuNode.SetScale(temp.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "minsuhu")
            {
                temp.skala -= 0.01f;
                suhuNode.SetScale(temp.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Temperature")
            {
                temp.skala = 0.04f;
                suhuNode.SetScale(temp.skala);
                textTemp.SetScale(1);
                textTempRek.SetScale(1);
                textTempRek.Position = new Vector3(0, -0.3f, 0);
                Debug.WriteLine(result.Value.Node.Name);
            }
            // Kelembapan
            else if (result != null && result.Value.Node.Name == "plushum")
            {
                hum.skala += 0.01f;
                lembapNode.SetScale(hum.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "minhum")
            {
                hum.skala -= 0.01f;
                lembapNode.SetScale(hum.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Humidity")
            {
                ui.eraseUI();
                hum.skala = 0.04f;
                textHum.SetScale(1);
                textHumRek.SetScale(1);
                textHumRek.Position = new Vector3(0, -0.3f, 0);
                lembapNode.SetScale(hum.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Intensitas Cahaya
            else if (result != null && result.Value.Node.Name == "pluschy")
            {
                intens.skala += 0.01f;
                cahayaNode.SetScale(intens.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "minchy")
            {
                intens.skala -= 0.01f;
                cahayaNode.SetScale(intens.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Light Intensity")
            {
                intens.skala = 0.04f;
                textIntens.SetScale(1);
                textIntensRek.SetScale(1);
                textIntensRek.Position = new Vector3(0, -0.3f, 0);
                cahayaNode.SetScale(intens.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Kebisingan Ruang
            else if (result != null && result.Value.Node.Name == "plusnoise")
            {
                noise.skala += 0.01f;
                bisingNode.SetScale(noise.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "minnoise")
            {
                noise.skala -= 0.01f;
                bisingNode.SetScale(noise.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Noise")
            {
                noise.skala = 0.04f;
                bisingNode.SetScale(noise.skala);
                textNoise.SetScale(1);
                textNoiseRek.SetScale(1);
                textNoiseRek.Position = new Vector3(0, -0.3f, 0);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Node Lampu
            else if (result != null && result.Value.Node.Name == "pluslampu")
            {
                lampu.skala += 0.01f;
                textjudullampu.SetScale(lampu.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "minlampu")
            {
                lampu.skala -= 0.01f;
                textjudullampu.SetScale(lampu.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Lampu")
            {
                lampu.skala = 0.06f;
                textjudullampu.SetScale(lampu.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Node Kipas
            else if (result != null && result.Value.Node.Name == "pluskipas")
            {
                kipas.skala += 0.01f;
                textjudulkipas.SetScale(kipas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "minkipas")
            {
                kipas.skala -= 0.01f;
                textjudulkipas.SetScale(kipas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Kipas")
            {
                kipas.skala = 0.06f;
                textjudulkipas.SetScale(kipas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Node Masukan Mode Kelas
            else if (result != null && result.Value.Node.Name == "pluskelas")
            {
                kelas.skala += 0.01f;
                PilihanModeKelas.SetScale(kelas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "minkelas")
            {
                kelas.skala -= 0.01f;
                PilihanModeKelas.SetScale(kelas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Pilihan Kelas")
            {
                kelas.skala = 0.06f;
                PilihanModeKelas.SetScale(kelas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null)
            {
                Debug.WriteLine(result.Value.Node.Name);
            }
            else
            {
                Debug.WriteLine("Kosong");
                Debug.WriteLine(HeadPosition.X);
                Debug.WriteLine(HeadPosition.Y);
            }

        }

        public override void OnGestureManipulationStarted()
        {
            base.OnGestureManipulationStarted();
            Ray cameraRay = RightCamera.GetScreenRay(0.5f, 0.5f);

            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);


            // Untuk Mengatur Pergerakan UI
            // Bagian Suhu
            if (result != null && (result.Value.Node.Name == "upsuhu"))
            {
                temp.y += 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightsuhu")
            {
                temp.x += 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downsuhu")
            {
                temp.y -= 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftsuhu")
            {
                temp.x -= 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontsuhu")
            {
                temp.z += 0.1f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backsuhu")
            {
                temp.z -= 0.1f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null && result.Value.Node.Name == "rotate_pos_noise")
            {
                noise.r += 10;
                bisingNode.Rotation = (Quaternion.FromAxisAngle(new Vector3(0, 1, 0), noise.r));
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rotate_neg_noise")
            {
                noise.r -= 10;
                bisingNode.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), noise.r);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Kelembapan
            else if (result != null && (result.Value.Node.Name == "uphum"))
            {
                hum.y += 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "righthum")
            {
                hum.x += 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downhum")
            {
                hum.y -= 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "lefthum")
            {
                hum.x -= 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "fronthum")
            {
                hum.z += 0.1f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backhum")
            {
                hum.z -= 0.1f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }


            // Bagian Intensitas Cahaya
            else if (result != null && (result.Value.Node.Name == "upchy"))
            {
                intens.y += 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightchy")
            {
                intens.x += 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downchy")
            {
                intens.y -= 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftchy")
            {
                intens.x -= 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontchy")
            {
                intens.z += 0.1f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backchy")
            {
                intens.z -= 0.1f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Kebisingan Suara
            else if (result != null && (result.Value.Node.Name == "upnoise"))
            {
                noise.y += 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightnoise")
            {
                noise.x += 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downnoise")
            {
                noise.y -= 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftnoise")
            {
                noise.x -= 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontnoise")
            {
                noise.z += 0.1f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backnoise")
            {
                noise.z -= 0.1f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Kontrol Lampu
            else if (result != null && (result.Value.Node.Name == "uplampu"))
            {
                lampu.y += 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightlampu")
            {
                lampu.x += 0.0011f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downlampu")
            {
                lampu.y -= 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftlampu")
            {
                lampu.x -= 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontlampu")
            {
                lampu.z += 0.1f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backlampu")
            {
                lampu.z -= 0.1f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Lampu")
            {
                lampu.x = 0.15f; lampu.y = 0.1f; lampu.z = 1; lampu.skala = 0.06f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                textjudullampu.SetScale(lampu.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Kontrol Kipas
            else if (result != null && (result.Value.Node.Name == "upkipas"))
            {
                kipas.y += 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightkipas")
            {
                kipas.x += 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downkipas")
            {
                kipas.y -= 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftkipas")
            {
                kipas.x -= 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontkipas")
            {
                kipas.z += 0.1f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backkipas")
            {
                kipas.z -= 0.1f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Pilihan Kelas

            else if (result != null && (result.Value.Node.Name == "upkelas"))
            {
                kelas.y += 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightkelas")
            {
                kelas.x += 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downkelas")
            {
                kelas.y -= 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftkelas")
            {
                kelas.x -= 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontkelas")
            {
                kelas.z += 0.1f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backkelas")
            {
                kelas.z -= 0.1f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Input Jarak
            else if (result != null && (result.Value.Node.Name == "upjarak"))
            {
                jarak.y += 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightjarak")
            {
                jarak.x += 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downjarak")
            {
                jarak.y -= 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftjarak")
            {
                jarak.x -= 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontjarak")
            {
                jarak.z += 0.1f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backjarak")
            {
                jarak.z -= 0.1f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null)
            {
                Debug.WriteLine(result.Value.Node.Name);
            }
            else
            {
                Debug.WriteLine("Kosong");
                Debug.WriteLine(HeadPosition.X);
                Debug.WriteLine(HeadPosition.Y);
            }
        }

        public override void OnGestureManipulationUpdated(Vector3 relativeHandPosition)
        {
            base.OnGestureManipulationUpdated(relativeHandPosition);
            Ray cameraRay = RightCamera.GetScreenRay(0.5f, 0.5f);

            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);

            if (result != null && result.Value.Node.Name == "lampu1plus")
            {
                if (slider1plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider1plus.y += 0.01f;
                    slider1min.y += 0.01f;
                    nlampu1 += 1;
                }
                Debug.WriteLine(nlampu1);
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu1min")
            {
                if (slider1min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider1plus.y -= 0.01f;
                    slider1min.y -= 0.01f;
                    nlampu1 -= 1;
                }
                Debug.WriteLine(nlampu1);
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu2plus")
            {
                if (slider2plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider2plus.y += 0.01f;
                    slider2min.y += 0.01f;
                    nlampu2 += 1;
                }
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu2min")
            {
                if (slider2min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider2plus.y -= 0.01f;
                    slider2min.y -= 0.01f;
                    nlampu2 -= 1;
                }
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu3plus")
            {
                if (slider3plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider3plus.y += 0.01f;
                    slider3min.y += 0.01f;
                    nlampu3 += 1;
                }
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu3min")
            {
                if (slider3min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider3plus.y -= 0.01f;
                    slider3min.y -= 0.01f;
                    nlampu3 -= 1;
                }
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu4plus")
            {
                if (slider4plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider4plus.y += 0.01f;
                    slider4min.y += 0.01f;
                    nlampu4 += 1;
                }
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu4min")
            {
                if (slider4min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider4plus.y -= 0.01f;
                    slider4min.y -= 0.01f;
                    nlampu4 -= 1;
                }
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas1plus")
            {
                if (slider5plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Kipas();
                }
                else
                {
                    slider5plus.y += 0.01f;
                    slider5min.y += 0.01f;
                    nkipas1 += 1;
                }
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas1min")
            {
                if (slider5min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Kipas();
                }
                else
                {
                    slider5plus.y -= 0.01f;
                    slider5min.y -= 0.01f;
                    nkipas1 -= 1;
                }
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas2plus")
            {
                if (slider6plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Kipas();
                }
                else
                {
                    slider6plus.y += 0.01f;
                    slider6min.y += 0.01f;
                    nkipas2 += 1;
                }
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas2min")
            {
                if (slider6min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Kipas();
                }
                else
                {
                    slider6plus.y -= 0.01f;
                    slider6min.y -= 0.01f;
                    nkipas2 -= 1;
                }
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }

            // Untuk Mengatur Pergerakan UI
            // Bagian Suhu
            else if (result != null && (result.Value.Node.Name == "upsuhu"))
            {
                temp.y += 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightsuhu")
            {
                temp.x += 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downsuhu")
            {
                temp.y -= 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftsuhu")
            {
                temp.x -= 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontsuhu")
            {
                temp.z += 0.1f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backsuhu")
            {
                temp.z -= 0.1f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }


            // Bagian Kelembapan
            else if (result != null && (result.Value.Node.Name == "uphum"))
            {
                hum.y += 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "righthum")
            {
                hum.x += 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downhum")
            {
                hum.y -= 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "lefthum")
            {
                hum.x -= 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "fronthum")
            {
                hum.z += 0.1f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backhum")
            {
                hum.z -= 0.1f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }


            // Bagian Intensitas Cahaya
            else if (result != null && (result.Value.Node.Name == "upchy"))
            {
                intens.y += 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightchy")
            {
                intens.x += 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downchy")
            {
                intens.y -= 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftchy")
            {
                intens.x -= 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontchy")
            {
                intens.z += 0.1f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backchy")
            {
                intens.z -= 0.1f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Kebisingan Suara
            else if (result != null && (result.Value.Node.Name == "upnoise"))
            {
                noise.y += 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightnoise")
            {
                noise.x += 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downnoise")
            {
                noise.y -= 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftnoise")
            {
                noise.x -= 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontnoise")
            {
                noise.z += 0.1f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backnoise")
            {
                noise.z -= 0.1f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Kontrol Lampu
            else if (result != null && (result.Value.Node.Name == "uplampu"))
            {
                lampu.y += 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightlampu")
            {
                lampu.x += 0.0011f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downlampu")
            {
                lampu.y -= 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftlampu")
            {
                lampu.x -= 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontlampu")
            {
                lampu.z += 0.1f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backlampu")
            {
                lampu.z -= 0.1f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Lampu")
            {
                lampu.x = 0.15f; lampu.y = 0.1f; lampu.z = 1; lampu.skala = 0.06f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                textjudullampu.SetScale(lampu.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Kontrol Kipas
            else if (result != null && (result.Value.Node.Name == "upkipas"))
            {
                kipas.y += 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightkipas")
            {
                kipas.x += 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downkipas")
            {
                kipas.y -= 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftkipas")
            {
                kipas.x -= 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontkipas")
            {
                kipas.z += 0.1f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backkipas")
            {
                kipas.z -= 0.1f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Pilihan Kelas

            else if (result != null && (result.Value.Node.Name == "upkelas"))
            {
                kelas.y += 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightkelas")
            {
                kelas.x += 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downkelas")
            {
                kelas.y -= 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftkelas")
            {
                kelas.x -= 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontkelas")
            {
                kelas.z += 0.1f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backkelas")
            {
                kelas.z -= 0.1f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null && (result.Value.Node.Name == "upjarak"))
            {
                jarak.y += 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightjarak")
            {
                jarak.x += 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downjarak")
            {
                jarak.y -= 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftjarak")
            {
                jarak.x -= 0.001f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontjarak")
            {
                jarak.z += 0.1f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backjarak")
            {
                jarak.z -= 0.1f;
                textjuduljarak.Position = new Vector3(jarak.x, jarak.y, jarak.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null)
            {
                Debug.WriteLine(result.Value.Node.Name);
            }
            else
            {
                Debug.WriteLine("Kosong");
                Debug.WriteLine(HeadPosition.X);
                Debug.WriteLine(HeadPosition.Y);
            }
        }

        public override void OnGestureManipulationCompleted(Vector3 relativeHandPosition)
        {
            base.OnGestureManipulationCompleted(relativeHandPosition);
            Ray cameraRay = RightCamera.GetScreenRay(0.5f, 0.5f);

            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);


            // Bagian Kontrol Lampu
            if (result != null && result.Value.Node.Name == "lampu1plus")
            {
                if (slider1plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider1plus.y += 0.01f;
                    slider1min.y += 0.01f;
                    nlampu1 += 1;
                }
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu1min")
            {
                if (slider1min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider1plus.y -= 0.01f;
                    slider1min.y -= 0.01f;
                    nlampu1 -= 1;
                }
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu2plus")
            {
                if (slider2plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider2plus.y += 0.01f;
                    slider2min.y += 0.01f;
                    nlampu2 += 1;
                }
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu2min")
            {
                if (slider2min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider2plus.y -= 0.01f;
                    slider2min.y -= 0.01f;
                    nlampu2 -= 1;
                }
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu3plus")
            {
                if (slider3plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider3plus.y += 0.01f;
                    slider3min.y += 0.01f;
                    nlampu3 += 1;
                }
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu3min")
            {
                if (slider3min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider3plus.y -= 0.01f;
                    slider3min.y -= 0.01f;
                    nlampu3 -= 1;
                }
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu4plus")
            {
                if (slider4plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Lampu();
                }
                else
                {
                    slider4plus.y += 0.01f;
                    slider4min.y += 0.01f;
                    nlampu4 += 1;
                }
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (result != null && result.Value.Node.Name == "lampu4min")
            {
                if (slider4min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Lampu();
                }
                else
                {
                    slider4plus.y -= 0.01f;
                    slider4min.y -= 0.01f;
                    nlampu4 -= 1;
                }
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas1plus")
            {
                if (slider5plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Kipas();
                }
                else
                {
                    slider5plus.y += 0.01f;
                    slider5min.y += 0.01f;
                    nkipas1 += 1;
                }
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas1min")
            {
                if (slider5min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Kipas();
                }
                else
                {
                    slider5plus.y -= 0.01f;
                    slider5min.y -= 0.01f;
                    nkipas1 -= 1;
                }
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas2plus")
            {
                if (slider6plus.y >= -0.58f)
                {
                    PesanError_NilaiAktuatorDiatasBatas_Kipas();
                }
                else
                {
                    slider6plus.y += 0.01f;
                    slider6min.y += 0.01f;
                    nkipas2 += 1;
                }
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }
            else if (result != null && result.Value.Node.Name == "kipas2min")
            {
                if (slider6min.y <= -1.57f)
                {
                    PesanError_NilaiAktuatorDibawahBatas_Kipas();
                }
                else
                {
                    slider6plus.y -= 0.01f;
                    slider6min.y -= 0.01f;
                    nkipas2 -= 1;
                }
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }

            // Untuk Mengatur Pergerakan UI
            // Bagian Suhu
            else if (result != null && (result.Value.Node.Name == "upsuhu"))
            {
                temp.y += 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightsuhu")
            {
                temp.x += 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downsuhu")
            {
                temp.y -= 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftsuhu")
            {
                temp.x -= 0.001f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontsuhu")
            {
                temp.z += 0.1f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backsuhu")
            {
                temp.z -= 0.1f;
                suhuNode.Position = new Vector3(temp.x, temp.y, temp.z);
                Debug.WriteLine(result.Value.Node.Name);
            }


            // Bagian Kelembapan
            else if (result != null && (result.Value.Node.Name == "uphum"))
            {
                hum.y += 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "righthum")
            {
                hum.x += 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downhum")
            {
                hum.y -= 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "lefthum")
            {
                hum.x -= 0.001f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "fronthum")
            {
                hum.z += 0.1f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backhum")
            {
                hum.z -= 0.1f;
                lembapNode.Position = new Vector3(hum.x, hum.y, hum.z);
                Debug.WriteLine(result.Value.Node.Name);
            }


            // Bagian Intensitas Cahaya
            else if (result != null && (result.Value.Node.Name == "upchy"))
            {
                intens.y += 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightchy")
            {
                intens.x += 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downchy")
            {
                intens.y -= 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftchy")
            {
                intens.x -= 0.001f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontchy")
            {
                intens.z += 0.1f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backchy")
            {
                intens.z -= 0.1f;
                cahayaNode.Position = new Vector3(intens.x, intens.y, intens.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Kebisingan Suara
            else if (result != null && (result.Value.Node.Name == "upnoise"))
            {
                noise.y += 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightnoise")
            {
                noise.x += 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downnoise")
            {
                noise.y -= 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftnoise")
            {
                noise.x -= 0.001f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontnoise")
            {
                noise.z += 0.1f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backnoise")
            {
                noise.z -= 0.1f;
                bisingNode.Position = new Vector3(noise.x, noise.y, noise.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Kontrol Lampu
            else if (result != null && (result.Value.Node.Name == "uplampu"))
            {
                lampu.y += 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightlampu")
            {
                lampu.x += 0.0011f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downlampu")
            {
                lampu.y -= 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftlampu")
            {
                lampu.x -= 0.001f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontlampu")
            {
                lampu.z += 0.1f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backlampu")
            {
                lampu.z -= 0.1f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Lampu")
            {
                lampu.x = 0.15f; lampu.y = 0.1f; lampu.z = 1; lampu.skala = 0.06f;
                textjudullampu.Position = new Vector3(lampu.x, lampu.y, lampu.z);
                textjudullampu.SetScale(lampu.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Kontrol Kipas
            else if (result != null && (result.Value.Node.Name == "upkipas"))
            {
                kipas.y += 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightkipas")
            {
                kipas.x += 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downkipas")
            {
                kipas.y -= 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftkipas")
            {
                kipas.x -= 0.001f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontkipas")
            {
                kipas.z += 0.1f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backkipas")
            {
                kipas.z -= 0.1f;
                textjudulkipas.Position = new Vector3(kipas.x, kipas.y, kipas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            // Bagian Panel Pilihan Kelas

            else if (result != null && (result.Value.Node.Name == "upkelas"))
            {
                kelas.y += 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "rightkelas")
            {
                kelas.x += 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "downkelas")
            {
                kelas.y -= 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "leftkelas")
            {
                kelas.x -= 0.001f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "frontkelas")
            {
                kelas.z += 0.1f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "backkelas")
            {
                kelas.z -= 0.1f;
                PilihanModeKelas.Position = new Vector3(kelas.x, kelas.y, kelas.z);
                Debug.WriteLine(result.Value.Node.Name);
            }

            else if (result != null)
            {
                Debug.WriteLine(result.Value.Node.Name);
            }
            else
            {
                Debug.WriteLine("Kosong");
                Debug.WriteLine(HeadPosition.X);
                Debug.WriteLine(HeadPosition.Y);
            }
        }

        public override async void OnGestureDoubleTapped()
        {
            base.OnGestureDoubleTapped();

            Ray cameraRay = RightCamera.GetScreenRay(0.5f, 0.5f);

            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);

            // Untuk Mengatur Pergerakan UI
            // Bagian Suhu
            if (result != null && (result.Value.Node.Name == "Temperature"))
            {
                temp.skala = 0.01f;
                suhuNode.SetScale(temp.skala);
                textTemp.SetScale(5);
                textTempRek.SetScale(5);
                textTempRek.Position = new Vector3(0, -0.7f, 0);
                Debug.WriteLine(result.Value.Node.Name);

            }
            else if (result != null && result.Value.Node.Name == "Humidity")
            {
                hum.skala = 0.01f;
                lembapNode.SetScale(hum.skala);
                lembapNode.SetScale(temp.skala);
                textHum.SetScale(5);
                textHumRek.SetScale(5);
                textHumRek.Position = new Vector3(0, -0.7f, 0);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Light Intensity")
            {
                intens.skala = 0.01f;
                cahayaNode.SetScale(intens.skala);
                cahayaNode.SetScale(temp.skala);
                textIntens.SetScale(5);
                textIntensRek.SetScale(5);
                textIntensRek.Position = new Vector3(0, -0.7f, 0);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Noise")
            {
                noise.skala = 0.01f;
                bisingNode.SetScale(noise.skala);
                bisingNode.SetScale(temp.skala);
                textNoise.SetScale(5);
                textNoiseRek.SetScale(5);
                textNoiseRek.Position = new Vector3(0, -0.7f, 0);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Lampu")
            {
                lampu.skala = 0.01f;
                textjudullampu.SetScale(lampu.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Kontrol Kipas")
            {
                kipas.skala = 0.01f;
                textjudulkipas.SetScale(kipas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null && result.Value.Node.Name == "Pilihan Kelas")
            {
                kelas.skala = 0.01f;
                PilihanModeKelas.SetScale(kelas.skala);
                Debug.WriteLine(result.Value.Node.Name);
            }
            else if (result != null)
            {
                Debug.WriteLine(result.Value.Node.Name);
            }
            else
            {
                Debug.WriteLine("Kosong");
            }
        }


        // Fungsi Menerima Rekomendasi
        void textRekSuhu(string a)
        {
            //textTempRek = suhuNode.CreateChild();
            var text3D1R = textTempRek.GetComponent<Text3D>();
            text3D1R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D1R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D1R.Text = "Rekomendasi : " + a;
            text3D1R.SetFont(CoreAssets.Fonts.AnonymousPro, 16);
            text3D1R.SetColor(Color.White);
        }

        void textRekHum(string a)
        {
            //textHumRek = lembapNode.CreateChild();
            var text3D2R = textHumRek.GetComponent<Text3D>();
            text3D2R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D2R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D2R.Text = "Rekomendasi : " + a;
            text3D2R.SetFont(CoreAssets.Fonts.AnonymousPro, 16);
            text3D2R.SetColor(Color.White);
        }

        void textRekIntens(string a)
        {
            //textIntensRek = cahayaNode.CreateChild();
            var text3D3R = textIntensRek.GetComponent<Text3D>();
            text3D3R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D3R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D3R.Text = "Rekomendasi : " + a;
            text3D3R.SetFont(CoreAssets.Fonts.AnonymousPro, 16);
            text3D3R.SetColor(Color.White);
        }

        void textRekNoise(string a)
        {
            //textNoiseRek = bisingNode.CreateChild();
            var text3D4R = textNoiseRek.GetComponent<Text3D>();
            text3D4R.HorizontalAlignment = HorizontalAlignment.Center;
            text3D4R.VerticalAlignment = VerticalAlignment.Top;
            // text3D1.ViewMask = 0x80000000; //hide from raycasts
            text3D4R.Text = "Rekomendasi : " + a;
            text3D4R.SetFont(CoreAssets.Fonts.AnonymousPro, 16);
            text3D4R.SetColor(Color.White);
        }

        // Fungsi Menerima Voice Command
        public void VoiceCommand_Update(string a, string b)
        {
            if (b == "TUNJUKKAN STATUS RUANGAN\n")
            {
                ui.updateUI(dataJson.temperature, dataJson.humidity, dataJson.lightintensity, dataJson.soundlevel, nmode);
                KondisiRuang_2D = true;
            }
            else if (b == "MATIKAN STATUS RUANGAN\n")
            {
                ui.eraseUI();
                KondisiRuang_2D = false;
            }
            else if (b == "NYALAKAN LAMPU\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider1plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider1plus.y += 0.01f;
                        slider1min.y += 0.01f;
                        nlampu1 += 1;
                    }

                    if (slider2plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider2plus.y += 0.01f;
                        slider2min.y += 0.01f;
                        nlampu2 += 1;
                    }

                    if (slider3plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider3plus.y += 0.01f;
                        slider3min.y += 0.01f;
                        nlampu3 += 1;
                    }
                    if (slider4plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider4plus.y += 0.01f;
                        slider4min.y += 0.01f;
                        nlampu4 += 1;
                    }
                }
                Debug.WriteLine(nlampu1);
                kirimpesan(1);
                kirimpesan(2);
                kirimpesan(3);
                kirimpesan(4);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }

            else if (b == "MATIKAN LAMPU\n")
            {
                
                    slider1min.y = -1.57f;
                    slider1plus.y = -1.57f;
                    nlampu1 = 1000;
                    slider2min.y = -1.57f;
                    slider2plus.y = -1.57f;
                    nlampu2 = 1000;
                    slider3min.y = -1.57f;
                    slider3plus.y = -1.57f;
                    nlampu3 = 2000;
                    slider4min.y = -1.57f;
                    slider4plus.y = -1.57f;
                    nlampu4 = 2000;
                
                kirimpesan(1);
                kirimpesan(2);
                kirimpesan(3);
                kirimpesan(4);
                kirimpesan(1);
                kirimpesan(2);
                kirimpesan(3);
                kirimpesan(4);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }

            //else if (b == "AKTIFKAN MODE HEMAT ENERGI\n")
            //{
            //    for (int i = 1; i <= 3; i++)
            //    {
            //        slider1min.y = -1.57f;
            //        nlampu1 = 0;
            //        slider4min.y = -1.57f;
            //        nlampu4 = 0;
            //    }
            //    kirimpesan(1);
            //    kirimpesan(4);

            //    lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
            //    lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);
            //    lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
            //    lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);

            //}

            else if (b == "NYALAKAN LAMPU ZONA SATU\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider1plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider1plus.y += 0.01f;
                        slider1min.y += 0.01f;
                        nlampu1 += 1;
                    }
                }
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);

            }

            else if (b == "NYALAKAN LAMPU ZONA DUA\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider2plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider2plus.y += 0.01f;
                        slider2min.y += 0.01f;
                        nlampu2 += 1;
                    }
                }
                kirimpesan(2);
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);

            }

            else if (b == "NYALAKAN LAMPU ZONA TIGA\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider3plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider3plus.y += 0.01f;
                        slider3min.y += 0.01f;
                        nlampu3 += 1;
                    }
                }
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);

            }

            else if (b == "NYALAKAN LAMPU ZONA EMPAT\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider4plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider4plus.y += 0.01f;
                        slider4min.y += 0.01f;
                        nlampu4 += 1;
                    }
                }
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);

            }

            else if (b == "MATIKAN LAMPU ZONA SATU\n")
            {
               
                    slider1min.y = -1.57f;
                    nlampu1 = 1000;
                slider1plus.y = -1.57f;
                kirimpesan(1);
                kirimpesan(1);
                kirimpesan(1);
                lampu1plus.Position = new Vector3(slider1plus.x, slider1plus.y, slider1plus.z);
                lampu1min.Position = new Vector3(slider1min.x, slider1min.y, slider1min.z);

            }

            else if (b == "MATIKAN LAMPU ZONA DUA\n")
            {
                
                    slider2min.y = -1.57f;
                    nlampu2 = 1000;
                slider2plus.y = -1.57f;
                kirimpesan(2);
                kirimpesan(2); 
                lampu2plus.Position = new Vector3(slider2plus.x, slider2plus.y, slider2plus.z);
                lampu2min.Position = new Vector3(slider2min.x, slider2min.y, slider2min.z);

            }

            else if (b == "MATIKAN LAMPU ZONA TIGA\n")
            {
                
                    slider3min.y = -1.57f;
                    nlampu3 = 2000;
                slider3plus.y = -1.57f;
                kirimpesan(3);
                lampu3plus.Position = new Vector3(slider3plus.x, slider3plus.y, slider3plus.z);
                lampu3min.Position = new Vector3(slider3min.x, slider3min.y, slider3min.z);

            }

            else if (b == "MATIKAN LAMPU ZONA EMPAT\n")
            {
                
                    slider4min.y = -1.57f;
                    nlampu4 = 2000;
                slider4plus.y = -1.57f;
                kirimpesan(4);
                lampu4plus.Position = new Vector3(slider4plus.x, slider4plus.y, slider4plus.z);
                lampu4min.Position = new Vector3(slider4min.x, slider4min.y, slider4min.z);
            }
            else if (b == "NYALAKAN KIPAS\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider5plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider5plus.y += 0.01f;
                        slider5min.y += 0.01f;
                        nkipas1 += 1;
                       
                      
                    }
                    if (slider6plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider6plus.y += 0.01f;
                        slider6min.y += 0.01f;
                      
                        
                        nkipas2 += 1;
                    }
                }
                Debug.WriteLine(nlampu1);
                kirimpesan(5);
                kirimpesan(6);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);

            }

            else if (b == "MATIKAN KIPAS\n")
            {
                slider5min.y = -1.57f;
                slider5plus.y = -1.57f;
                nkipas1 = 0;
                slider6min.y = -1.57f;
                slider6plus.y = -1.57f;
                nkipas2 = 0;
                kirimpesan(5);
                kirimpesan(6);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }

            else if (b == "NYALAKAN KIPAS ZONA SATU\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider5plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider5plus.y += 0.01f;
                        slider5min.y += 0.01f;
                        nkipas1 += 1;
                    }
                }
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);

            }

            else if (b == "MATIKAN KIPAS ZONA SATU\n")
            {
                slider5min.y = -1.57f;
                slider5plus.y = -1.57f;
                nkipas1 = 0;
                kirimpesan(5);
                kirimpesan(5);
                kirimpesan(5);
                kipas1plus.Position = new Vector3(slider5plus.x, slider5plus.y, slider5plus.z);
                kipas1min.Position = new Vector3(slider5min.x, slider5min.y, slider5min.z);
            }

            else if (b == "NYALAKAN KIPAS ZONA DUA\n")
            {
                for (int i = 1; i <= 50; i++)
                {
                    if (slider6plus.y >= -0.58f)
                    {
                        PesanError_NilaiAktuatorDiatasBatas_Lampu();
                    }
                    else
                    {
                        slider6plus.y += 0.01f;
                        slider6min.y += 0.01f;
                        nkipas2 += 1;
                    }
                }
                kirimpesan(6);
                kirimpesan(6);
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);

            }

            else if (b == "MATIKAN KIPAS ZONA DUA\n")
            {
                slider6min.y = -1.57f;
                slider6plus.y = -1.57f;
                nkipas2 = 0;
                kirimpesan(6);
                kirimpesan(6);
                kirimpesan(6);
                kipas2plus.Position = new Vector3(slider6plus.x, slider6plus.y, slider6plus.z);
                kipas2min.Position = new Vector3(slider6min.x, slider6min.y, slider6min.z);
            }


            else if (b == "AKTIFKAN SISTEM\n")
            {
                ui_Voice = new UITwoD_Voice(ref uiRoot);
                ui_Voice.updateUI(dataJson2.nama, dataJson2.perintah, dataJson2.jarak);
                VoiceCommand_2D = true;
            }


            else if (b == "TAMPILKAN EKSPRESI SISWA\n")
            {
                ui_Face.updateUI(dataJson3.nama, dataJson3.ekspresi);
                Face_2D = true;
            }
            else if (b == "SEMBUNYIKAN EKSPRESI SISWA\n")
            {
                ui_Face.eraseUI();
                Face_2D = false;
            }
            else if (b == "TAMPILKAN KAMERA\n")
            {
                ui_Gesture.updateUI(dataJson4.cameraID, dataJson4.gesture, dataJson4.confidence);
                Gesture_2D = true;
            }
            else if (b == "SEMBUNYIKAN KAMERA\n")
            {
                ui_Gesture.eraseUI();
                Gesture_2D = false;
            }
            else if (b == "MATIKAN SISTEM\n")
            {
                ui_Voice.eraseUI();
                Face_2D = false;
            }

        }
    }
}
