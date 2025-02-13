using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using PointLib;
//using System.Drawing;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Text;
using YamlDotNet.Serialization;

namespace Lab_1
{
    public partial class PointForm : Form
    {
        private Point[] points = null;

        public PointForm()
        {
            InitializeComponent();
        }

        private void PointForm_Load(object sender, EventArgs e)
        {

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();
            var a = new Point3D();
            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 != 0 ? new Point3D() : new Point();

            listBox.DataSource = points;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (points == null)
                return;
            Array.Sort(points);

            listBox.DataSource = null;
            listBox.DataSource = points;
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|YAML|*.yaml|Binary|*.bin|TONY|*.tony";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        bf.Serialize(fs, points);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        sf.Serialize(fs, points);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        xf.Serialize(fs, points);
                        break;
                    case ".json":
                        var jf = new JsonSerializer
                        {
                            TypeNameHandling = TypeNameHandling.All
                        };

                        using (var w = new StreamWriter(fs))
                            jf.Serialize(w, points);
                        break;
                    case ".yaml":
                        var serializer = new SerializerBuilder()
                            .WithTagMapping("!point", typeof(Point))
                            .WithTagMapping("!point_3d", typeof(Point3D))
                            .Build();
                        using (var writer = new StreamWriter(fs, Encoding.UTF8))
                            serializer.Serialize(writer, points);
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|YAML|*.yaml|Binary|*.bin|TONY|*.tony";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        points = (Point[])bf.Deserialize(fs);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        points = (Point[])sf.Deserialize(fs);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        points = (Point[])xf.Deserialize(fs);
                        break;
                    case ".json":
                        var jf = new JsonSerializer
                        {
                            TypeNameHandling = TypeNameHandling.All
                        };

                        using (var r = new StreamReader(fs))
                            points = (Point[])jf.Deserialize(r, typeof(Point[]));
                        break;
                    case ".yaml":
                        var deserializer = new DeserializerBuilder()
                            .WithTagMapping("!point", typeof(Point))
                            .WithTagMapping("!point_3d", typeof(Point3D))
                            .IgnoreUnmatchedProperties()
                            .Build();
                        using (var reader = new StreamReader(fs, Encoding.UTF8))
                            points = deserializer.Deserialize<Point[]>(reader);
                        break;
                }
                listBox.DataSource = null;
                listBox.DataSource = points;

            }
        }
    }
}