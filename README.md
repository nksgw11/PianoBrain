# PianoBrain
> A program to assist piano practitioners in learning chords and scales
>

![image-20200328181627108](https://github.com/nksgw11/PianoBrain/raw/master/README.assets/image-20200328181627108.png)

一款基于.Net Core 3.0开发的钢琴和弦辅助WPF程序，提供和弦识别，音阶显示，和弦显示与调式识别功能。以下是使用到的第三方类库：

- NAudio ：https://github.com/naudio/NAudio
- Locrian ：基于.Net Core 3.0开发的音乐理论框架（暂未开源，还在开发当中）

## **下载地址**

打包好的Windows程序（PianoBrain_win_64）在这个地址里：https://github.com/nksgw11/PianoBrain/releases/tag/v1.0.0

该程序需要依赖于.Net Core 3.0的框架，如果需要独立不依赖框架可以选择**PianoBrain_win_64_dotnet_core_3.0**版本程序。

## **主要功能**

![image-20200328184839711](https://github.com/nksgw11/PianoBrain/tree/master/README.assets/image-20200328184839711.png)

主窗口主要分以上四个区域：

- 1号区域：和弦识别设置参数区
- 2号区域：音阶显示设置参数与调式识别区
- 3号区域： 和弦音符与琶音显示设置区
- 4号区域：琴键指定区（如果鼠标在琴键上停留将会在左侧边缘显示该琴键对应的音符名）

> 琴键指定方法：使用鼠标在需要选中的琴键上单击，琴键会变成橘色，表示已被选中，如果想取消单个或所有选中的琴键，直接在任意一个橘色琴键上双击即可

### **1. 钢琴和弦识别**

使用和弦识别功能前需要提前指定多个琴键，即在4号区域出现多个橘色琴键，表示已选中所要识别的和弦音。在1号区域主要有两个重要的参数：

- 代号风格（已废弃）：主要有两种代号风格，第一种是PerferSharp模式，该种模式将主要使用#来表示变音；第二种是PerferFlat模式，该种模式将主要使用b来表示变音。
- 根音模式：主要有两种模式，默认为指定根音，在该种模式下，默认将**选中的琴键**中最低音（即琴键上最靠左侧位置）作为预测和弦的bass音；另一种模式将不指定确定的bass音。

Locrian内置的和弦预测函数可以对指定的音符进行深度搜索可能的和弦代号，包括**各种三和弦、七和弦、九和弦、十一和弦、十三和弦、挂二和弦、挂四和弦、六和弦、增和弦、减和弦和各种变化音组合**。最新已支持到各类**转位和弦**。

![image-20200328191134213](https://github.com/nksgw11/PianoBrain/tree/master/README.assets/image-20200328191134213.png)

点击和弦识别按钮后，将会在中心区域显示出和弦名称代号，Locrian的和弦预测函数通常都能够给出非常多种和弦代号的叫法，默认显示的为通过内部评分算法给出的最佳和弦，点击左右两侧的箭头将显示更多其他可能的和弦代号。

### **2. 音阶显示与调式识别**

**<u>音阶显示</u>**：通过设定主音和音阶类型选项，点击键位显示即可在4号区域显示出所有该音阶下的音符，双击可取消所有选中。

![image-20200328201059890](https://github.com/nksgw11/PianoBrain/tree/master/README.assets/image-20200328201059890.png)

Locrian音阶类目前支持多种不同的调式，如**中古调式**、**布鲁斯音阶**等多种类型。支持的所有调式选项包括：

- Half_Full
- Full_Half
- Nature_Major
- Dorian
- Phrygian
- Lydian
- Mixolydian
- Nature_Minor 
- Locrian
- Harmonic_Minor
- Harmonic_Major 
- Melodic_Minor 
- Melodic_Major 
- Mix_Blues
- Flamenco
- Alter
- Blues
- Whole_Tone
- Penta_Major
- Penta_Minor

**<u>调式识别</u>**：通过听曲识调按钮可以对音频文件进行识别调式，其主要调用的后台程序为libKeyFinder.NET.CLI，地址为https://github.com/aybe/libKeyFinder.NET.CLI，主要支持的音频文件类型为WAV, FLAC, Ogg/Vorbis。本程序通过NAudio的功能额外提供了对MP3文件的支持，**如果选择了后缀为.mp3文件格式，将会自动先在相同文件夹路径下创建同名WAV文件，然后再进行识别。**识别后的结果反馈将反馈给PianoBrain程序，主窗口将显示对应的调内音阶和音阶名。*<u>注意：在这个功能下只会识别出自然大调和自然小调，暂不支持其他多种调式。</u>*

![image-20200328202905087]https://github.com/nksgw11/PianoBrain/tree/master/README.assets/image-20200328202905087.png)

### **3. 和弦与琶音显示**

<u>**和弦显示**</u>：通过选定**主音**，**后缀名**、**变化音**和**根音**，点击和弦显示将在琴键上显示出和弦内音，其和弦配置为**根音+和弦内音**，默认显示**最低把位和弦按法**，点击左右两侧箭头将逐步切换为各种**转位或把位形式**。

![image-20200328203444368](https://github.com/nksgw11/PianoBrain/tree/master/README.assets/image-20200328203444368.png)

**<u>琶音显示</u>**：在选定和弦的基础上，点击琶音显示将展示钢琴键上所有的和弦内音。

## 反馈

如果有任何相关问题反馈，可以发邮件给作者：m14790619979@163.com
