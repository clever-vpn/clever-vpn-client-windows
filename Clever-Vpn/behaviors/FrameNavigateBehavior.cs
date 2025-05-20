using Clever_Vpn.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Clever_Vpn.behaviors
{
    class FrameNavigateBehavior : Behavior<Frame>
    {
        VpnViewModel vm { get; } = ((App)Application.Current).ViewModel;

        protected override void OnAttached()
        {
            base.OnAttached();
            vm.PropertyChanged += Vm_PropertyChanged;
            NavigateByActivateState();
        }

        protected override void OnDetaching()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
            base.OnDetaching();

        }

        private void Vm_PropertyChanged(object? s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VpnViewModel.ActivateState))
                NavigateByActivateState();
        }
  

        void NavigateByActivateState()
        {
            switch (vm.ActivateState)
            {
                case ActivationState.Activated:
                    Navigate(typeof(Pages.HomePage.HomePage), null);
                    break;
                default:
                    Navigate(typeof(Pages.ActivatePage.ActivatePage), null);
                    break;
            }

            AssociatedObject.BackStack.Clear();
        }

        public bool Navigate(Type pageType, object? parameter)
        {

            if (AssociatedObject.Content?.GetType() == pageType)
            {
                return false;
            }

            return AssociatedObject.Navigate(pageType, parameter, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        //public void GoBack()
        //{
        //    AssociatedObject.GoBack();
        //}

    }
}
