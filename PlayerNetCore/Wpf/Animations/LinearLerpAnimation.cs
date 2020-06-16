// https://stackoverflow.com/questions/1988421/smooth-animation-using-matrixtransform
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NekoPlayer.Wpf.Animations
{
    public class LinearLerpAnimation : AnimationTimeline
    {

        public double? From
        {
            set { SetValue(FromProperty, value); }
            get { return (double)GetValue(FromProperty); }
        }
        public static DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(double?), typeof(LinearLerpAnimation), new PropertyMetadata(null));

        public double? To
        {
            set { SetValue(ToProperty, value); }
            get { return (double)GetValue(ToProperty); }
        }
        public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(double?), typeof(LinearLerpAnimation), new PropertyMetadata(null));

        public LinearLerpAnimation()
        {
        }

        public LinearLerpAnimation(double from, double to, Duration duration)
        {
            Duration = duration;
            From = from;
            To = to;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
            {
                return null;
            }

            double progress = animationClock.CurrentProgress.Value;
            double from = From ?? (double)defaultOriginValue;

            if (To.HasValue)
            {
                double to = To.Value;
                double t = progress * progress;
                return from + (1 - t) * (to - from);
            }

            return 0.0;
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new LinearLerpAnimation();
        }

        public override System.Type TargetPropertyType
        {
            get { return typeof(Matrix); }
        }
    }
}