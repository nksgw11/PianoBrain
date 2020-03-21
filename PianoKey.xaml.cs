using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Locrian.Core;
using Locrian.Utils;
using Locrian.Algorithm;
using Microsoft.Win32;
using System.Diagnostics;

namespace WpfApp2Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> PianoKeyChooseNote = new List<string>();
        private List<Chord> candidate = new List<Chord>();
        private string ArrowMode = "";
        private List<int> ChordNoteNumTmp = new List<int>();
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            InitialUser();
        }

        private void InitialUser()
        {
            List<string> SUFFIX = Common.ChordSuffix.ToList();
            ChordSuffix.ItemsSource = SUFFIX;
            ChordSuffix.SelectedItem = "maj";
        }

        /// <summary>
        /// 自定义工具类
        /// </summary>
        public class Tools
        {
            //双击事件定时器
            private static DispatcherTimer _timer;
            //是否单击过一次
            private static bool _isFirst;

            static Tools()
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
                _timer.Tick += new EventHandler(_timer_Tick);
            }

            /// <summary>
            /// 判断是否双击
            /// </summary>
            /// <returns></returns>
            public static bool IsDoubleClick()
            {
                if (!_isFirst)
                {
                    _isFirst = true;
                    _timer.Start();
                    return false;
                }
                else
                {
                    return true;
                }
            }

            //间隔时间
            static void _timer_Tick(object sender, EventArgs e)
            {
                _isFirst = false;
                _timer.Stop();
            }
        }

        /// <summary>
        /// 点击钢琴键后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PianoKeyClickDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (Tools.IsDoubleClick())
            {
                foreach (var children in Pianokey.Children)
                {
                    Image img = children as Image;
                    ChangePianoKeyChooseStatus(img, status: "CancelChoose");
                }
                PianoKeyChooseNote = new List<string>();
                Left.Visibility = Visibility.Hidden;
                Right.Visibility = Visibility.Hidden;
            }
            else
            {
                Image img = sender as Image;
                string tag = img.Tag as string;
                var Original = img.Source.ToString().Split('.');
                string pre_str = Original[0];
                string suffix_str = Original[1];
                string New_Address = "";
                if (pre_str.Substring(pre_str.Length - 1) == "1")
                {
                    PianoKeyChooseNote.Remove(tag);
                    New_Address = $"{pre_str.Substring(0, pre_str.Length - 1)}.{suffix_str}";
                }
                else
                {
                    PianoKeyChooseNote.Add(tag);
                    New_Address = $"{pre_str}1.{suffix_str}";
                }
                //Console.WriteLine(New_Address);
                img.Source = new ImageSourceConverter().ConvertFromString(New_Address) as ImageSource;
            }
        }

        private void ChangePianoKeyChooseStatus(Image img, string status = "CancelChoose")
        {
            string tag = img.Tag as string;
            var Original = img.Source.ToString().Split('.');
            string pre_str = Original[0];
            string suffix_str = Original[1];
            string New_Address = "";
            if (status == "CancelChoose")
            {
                if (pre_str.Substring(pre_str.Length - 1) == "1")
                {
                    New_Address = $"{pre_str.Substring(0, pre_str.Length - 1)}.{suffix_str}";
                    img.Source = new ImageSourceConverter().ConvertFromString(New_Address) as ImageSource;
                    //Console.WriteLine(New_Address);
                }
            }
            else if (status == "Choose")
            {
                if (pre_str.Substring(pre_str.Length - 1) != "1")
                {
                    New_Address = $"{pre_str}1.{suffix_str}";
                    img.Source = new ImageSourceConverter().ConvertFromString(New_Address) as ImageSource;
                }
            }                   
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Left.Visibility = Visibility.Hidden;
            Right.Visibility = Visibility.Hidden;
            if (PianoKeyChooseNote.Count() == 0)
            {
                MessageBox.Show("未指定钢琴上的键位！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ArrowMode = "DetectChord";
                string style = PianoChordStyle.Text;
                string bassStyle = PianoChordBassStyle.Text;
                string bassNoteStr = PianoKeyChooseNote.First(s =>
                int.Parse(s.Split('-')[2]) == PianoKeyChooseNote.Min(k => int.Parse(k.Split('-')[2]))).Split('-')[1];
                List<Note> NoteList = new List<Note>();

                foreach (var keyNote in PianoKeyChooseNote)
                {
                    int index = int.Parse(keyNote.Split('-')[0]);
                    string noteStr = keyNote.Split('-')[1];
                    var note = NoteUtils.ParseNote(noteStr);
                    if (!NoteList.Contains(note))
                        NoteList.Add(note);
                }

                RetroChord res;
                try
                {
                    if (bassStyle == "不指定低音")
                        res = ChordAnalysis.ChordRecognizer(NoteList);
                    else
                        res = ChordAnalysis.ChordRecognizer(NoteList, bass: NoteUtils.ParseNote(bassNoteStr));

                
                    candidate = res.Select(r => r.chord).ToList();
                    ChordNameTextBox.Text = res.BestChord.Title;
                    Left.Visibility = Visibility.Visible;
                    Right.Visibility = Visibility.Visible;
                }
                catch
                {
                    MessageBox.Show("和弦分析出现未知错误！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TurnNextOneEvent(object sender, MouseButtonEventArgs e)
        {
            if (ArrowMode == "DetectChord")
            {
                Image img = sender as Image;
                string tag = img.Tag as string;
                string currentTitle = ChordNameTextBox.Text;
                int length = candidate.Count();
                int index = candidate.IndexOf(candidate.First(c => c.Title == currentTitle));

                if (tag == "right")
                {
                    if (index + 1 >= length)
                        ChordNameTextBox.Text = candidate[0].Title;
                    else
                        ChordNameTextBox.Text = candidate[index + 1].Title;
                }
                else
                {
                    if (index == 0)
                        ChordNameTextBox.Text = candidate[length - 1].Title;
                    else
                        ChordNameTextBox.Text = candidate[index - 1].Title;
                } 
            }
            else if (ArrowMode == "DisplayChord")
            {
                Image img = sender as Image;
                string tag = img.Tag as string;
                if (tag == "right")
                {
                    int second = ChordNoteNumTmp[1];
                    ChordNoteNumTmp.Remove(second);
                    ChordNoteNumTmp.Add(second + 12);
                    foreach (var children in Pianokey.Children)
                    {
                        Image img1 = children as Image;
                        string tag1 = img1.Tag as string;
                        int imgNum = int.Parse(tag1.Split('-')[2]);
                        if (imgNum == second)
                        {
                            ChangePianoKeyChooseStatus(img1, status: "CancelChoose");
                        }
                        if (imgNum == second + 12)
                        {
                            ChangePianoKeyChooseStatus(img1, status: "Choose");
                        }

                    }
                }
                else
                {
                    int last = ChordNoteNumTmp[ChordNoteNumTmp.Count()-1];
                    ChordNoteNumTmp.Remove(last);
                    ChordNoteNumTmp.Add(last - 12);
                    foreach (var children in Pianokey.Children)
                    {
                        Image img1 = children as Image;
                        string tag1 = img1.Tag as string;
                        int imgNum = int.Parse(tag1.Split('-')[2]);
                        if (imgNum == last)
                        {
                            ChangePianoKeyChooseStatus(img1, status: "CancelChoose");
                        }
                        if (imgNum == last - 12)
                        {
                            ChangePianoKeyChooseStatus(img1, status: "Choose");
                        }

                    }
                }
                ChordNoteNumTmp.Sort();
                CheckArrowVisibleState();
            }
        }

        private void EnterPianoKey(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            string tag = img.Tag as string;
            var res = tag.Split('-');
            DisplayNoteNameTextBox.Text = $"{res[1]}{res[0]}";
        }

        private void LeavePianoKey(object sender, MouseEventArgs e)
        {
            DisplayNoteNameTextBox.Text = string.Empty;
        }

        private void DisplayScaleClickEvent(object sender, RoutedEventArgs e)
        {
            ClearChoosePianoKey();
            string keyStr = ScaleKey.Text;
            string type = ScaleType.Text;

            ChordNameTextBox.Text = string.Empty;

            Scale targetScale = new Scale(NoteUtils.ParseNote(keyStr), Common.TryEnum<Tonic>(type));
            DisplayScaleCore(targetScale);
        }

        private void DisplayScaleCore(Scale targetScale)
        {
            try
            {
                var noteInScale = targetScale.GetElementsNote().ToList();

                foreach (var children in Pianokey.Children)
                {
                    Image img = children as Image;
                    string tag = img.Tag as string;
                    Note note = NoteUtils.ParseNote(tag.Split('-')[1]);
                    if (noteInScale.Contains(note))
                        ChangePianoKeyChooseStatus(img, status: "Choose");
                    else
                        ChangePianoKeyChooseStatus(img, status: "CancelChoose");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("音阶键位显示出现未知错误！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DetectKeyOfSong(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Title = "选择需要打开的音频文件";
            //openFileDialog.Filter = "音频文件(*.mp3)|*.mp3;*.wav;*.flac|所有文件(*.*)|*.*";
            //var result = openFileDialog.ShowDialog();
            //string path;
            //if (result == true)
            //{
            //    path = openFileDialog.FileName;
            KeyDetector();
            //}
            //else
            //    MessageBox.Show("未知错误！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void KeyDetector()
        {
            string exe_path = "";
            try
            {
                exe_path = @".\KeyFinder-WIN\KeyFinder.exe";  // 被调exe
                string[] the_args = new string[] { };   // 被调exe接受的参数
                StartProcess(exe_path, the_args);
            }
            catch (Exception)
            {
                MessageBox.Show("没有找到KeyFinder.exe！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public bool StartProcess(string runFilePath, params string[] args)
        {
            string s = "";
            foreach (string arg in args)
            {
                s = s + arg + " ";
            }
            s = s.Trim();
            Process process = new Process();//创建进程对象    
            ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath, s); // 括号里是(程序名,参数)
            process.StartInfo = startInfo;
            //process.StartInfo.UseShellExecute = true;    //是否使用操作系统的shell启动
            startInfo.RedirectStandardInput = true;      //接受来自调用程序的输入     
            startInfo.RedirectStandardOutput = true;     //由调用程序获取输出信息
            startInfo.CreateNoWindow = false;             //不显示调用程序的窗口 
            process.Start();
            return true;
        }

        private void ChordKeyChangedEvent(object sender, RoutedEventArgs e)
        {
            ChordBassKey.Text = ChordKey.Text;
        }

        private void DisplayChordAccordingToNameClickEvent(object sender, RoutedEventArgs e)
        {
            Chord target = GetChordByNameSet();
            var bass = target.BassNote;
            int bassNum = ((int)bass + 1);
            ChordNoteNumTmp.Add(bassNum);
            string bassTag = "1-" + NoteUtils.ToString(bass) + "-" + bassNum.ToString();
            var noteList = target.ToInternalNoteList().Where(n => n != null).Cast<InternalNote>().ToList();
            List<string> noteTags = new List<string>() { bassTag };
            noteList.ForEach(n =>
            {
                string level;
                string note = NoteUtils.ToString(n);
                string num;
                if ((int)n > (int)bass)
                {
                    level = "1";
                    num = (bassNum + (int)n - (int)bass).ToString();
                }
                else if ((int)n == (int)bass)
                {
                    level = "2";
                    num = (12 + bassNum).ToString();
                }
                else
                {
                    level = "2";
                    num = (12 + bassNum - (int)bass + (int)n).ToString();
                }
                noteTags.Add($"{level}-{note}-{num}");
                ChordNoteNumTmp.Add(int.Parse(num));
            });
            DisplayKeyCore(noteTags);
            ArrowMode = "DisplayChord";
            ChordNoteNumTmp.Sort();
            CheckArrowVisibleState();
        }

        private void CheckArrowVisibleState()
        {
            if (ArrowMode == "DisplayChord")
            {
                int second = ChordNoteNumTmp[1];
                int last = ChordNoteNumTmp[ChordNoteNumTmp.Count() - 1];
                foreach (var children in Pianokey.Children)
                {
                    Image img = children as Image;
                    string tag = img.Tag as string;
                    int imgNum = int.Parse(tag.Split('-')[2]);
                    if (imgNum == second)
                    {
                        if (imgNum + 12 > 48)
                            Right.Visibility = Visibility.Hidden;
                        else
                            Right.Visibility = Visibility.Visible;
                    }

                    if (imgNum == last)
                    {
                        if (imgNum - 12 <= ChordNoteNumTmp[0] || imgNum - 12 <= 0)
                            Left.Visibility = Visibility.Hidden;
                        else
                            Left.Visibility = Visibility.Visible;
                    }

                }
            }
        }

        private Chord GetChordByNameSet()
        {
            ClearChoosePianoKey();
            string keyString = ChordKey.Text;
            string suffix = ChordSuffix.Text;
            string variety = ChordVariety.Text == "无" ? "" : "(" + ChordVariety.Text + ")";
            string bassString = ChordBassKey.Text != keyString ? "/" + ChordBassKey.Text : "";

            string chordName = $"{keyString}{suffix}{variety}{bassString}";
            ChordNameTextBox.Text = chordName;

            Chord target = ChordsFactory.ImportChord(chordName);
            return target;
        }

        /// <summary>
        /// 清除所有选择的琴键
        /// </summary>
        private void ClearChoosePianoKey()
        {
            Left.Visibility = Visibility.Hidden;
            Right.Visibility = Visibility.Hidden;
            foreach (var children in Pianokey.Children)
            {
                Image img = children as Image;
                ChangePianoKeyChooseStatus(img, status: "CancelChoose");
            }
            ChordNoteNumTmp = new List<int>();
        }

        private void DisplayKeyCore(string tag)
        {
            try
            {
                foreach (var children in Pianokey.Children)
                {
                    Image img = children as Image;
                    string imgTag = img.Tag as string;
                    Note note = NoteUtils.ParseNote(imgTag.Split('-')[1]);
                    if (imgTag == tag)
                    {
                        ChangePianoKeyChooseStatus(img, status: "Choose");
                        break;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("和弦显示出现未知错误！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayKeyCore(IEnumerable<string> tagList)
        {
            foreach (var tag in tagList)
            {
                DisplayKeyCore(tag);
            }

        }

        private void DisplayChordArpeggioEvent(object sender, RoutedEventArgs e)
        {
            Left.Visibility = Visibility.Hidden;
            Right.Visibility = Visibility.Hidden;
            Chord target = GetChordByNameSet();
            var noteList = target.ToInternalNoteList().Where(n => n != null).Cast<InternalNote>().ToList();
            noteList.Add(target.BassNote);
            noteList.Distinct();
            foreach (var children in Pianokey.Children)
            {
                Image img = children as Image;
                string imgTag = img.Tag as string;
                string noteStr = imgTag.Split('-')[1];
                if (noteList.Any(n => noteStr == NoteUtils.ToString(n)))
                {
                    ChangePianoKeyChooseStatus(img, status: "Choose");
                }
            }

        }
    }
}
