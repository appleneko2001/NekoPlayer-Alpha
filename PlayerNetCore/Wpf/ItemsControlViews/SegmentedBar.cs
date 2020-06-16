using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace NekoPlayer.Wpf.ItemsControlViews
{
    public class SegmentTemplate
    {
        public SegmentTemplate(string text, System.Drawing.Color color, long data)
        {
            Text = text;
            Color = Color.FromRgb(color.R,color.G,color.B);
            Data = data;
        }
        public SegmentTemplate(string text, Color color, long data)
        {
            Text = text;
            Color = color;
            Data = data;
        }
        public string Text;
        public Color Color;
        public long Data;
    }
    public class SegmentPart : ViewModelBase
    {
        public SegmentPart(double percent, long data, int index, Color color, string text)
        {
            m_Percent = percent;
            m_Index = index;
            m_Text = text;
            m_Color = color;
            m_Data = data;
        }
        private double m_Percent;
        public double Percent => m_Percent;
        private int m_Index;
        public int Index => m_Index;
        private Color? m_Color;
        public Color? Color => m_Color;
        private string m_Text;
        public string Text => m_Text;
        private long m_Data;
        public long Data => m_Data;
        public void Destructor()
        {
            m_Percent = double.NaN;
            m_Index = 0;
            m_Color = null;
            m_Text = null;
            m_Data = 0;
        }
    }
    public class ProcessedSegment : ViewModelBase
    {
        /// <summary>
        /// Generate segments
        /// </summary>
        public ProcessedSegment(SegmentTemplate[] datas)
        {
            List<Tuple<double,long>> list = new List<Tuple<double, long>>();
            for(int i = 0; i< datas?.Length; i++)
            {
                long data = datas[i].Data;
                long remains = 0;
                for(int j = 0; j < datas?.Length; j++)
                {
                    remains += datas[j].Data;
                }
                double result = (double)data / (double)remains;
                list.Add(new Tuple<double, long>(result,data));
            }
            SegmentParts = new ObservableCollection<SegmentPart>();
            int index = 0;
            foreach (var item in list)
            {
                SegmentParts.Add(new SegmentPart(item.Item1,item.Item2, index, datas[index].Color, datas[index].Text)); 
                index++;
            }
        }
        public int SegmentsCount { get; private set; }
        public ObservableCollection<SegmentPart> SegmentParts { get; private set; }
    }
}
