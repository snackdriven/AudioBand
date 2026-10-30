using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AudioBand.Commands;
using Xunit;

namespace AudioBand.Test.Commands
{
    public class RelayCommandTests
    {
        [Fact]
        public void Execute_InvokesAction()
        {
            bool invoked = false;
            var command = new RelayCommand(() => invoked = true);

            command.Execute(null);

            Assert.True(invoked);
        }

        [Fact]
        public void RelayCommandT_Execute_InvokesActionWithParameter()
        {
            int result = 0;
            var command = new RelayCommand<int>(val => result = val);

            command.Execute(42);

            Assert.Equal(42, result);
        }

        [Fact]
        public void RelayCommand_CanExecute_ReturnsTrueWhenNoPredicate()
        {
            var command = new RelayCommand(() => { });

            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void RelayCommand_CanExecute_ReturnsPredicateResult()
        {
            bool canExecute = false;
            var command = new RelayCommand(() => { }, () => canExecute);

            Assert.False(command.CanExecute(null));

            canExecute = true;
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void RelayCommandT_CanExecute_ReturnsPredicateResult()
        {
            var command = new RelayCommand<int>(val => { }, val => val > 10);

            Assert.False(command.CanExecute(5));
            Assert.True(command.CanExecute(15));
        }

        [Fact]
        public void Constructor_NullExecute_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RelayCommand((Action)null));
            Assert.Throws<ArgumentNullException>(() => new RelayCommand<int>(null));
        }

        [Fact]
        public void Observe_INotifyPropertyChanged_AttachesHandlerAndRunsWithoutError()
        {
            var command = new RelayCommand(() => { });
            var mockSubject = new MockNotifyPropertyChanged();

            command.Observe(mockSubject, "TestProperty");

            mockSubject.RaisePropertyChanged("TestProperty");
            mockSubject.RaisePropertyChanged("OtherProperty");
        }

        [Fact]
        public void Observe_INotifyCollectionChanged_AttachesHandlerAndRunsWithoutError()
        {
            var command = new RelayCommand(() => { });
            var mockCollection = new MockNotifyCollectionChanged();

            command.Observe(mockCollection);

            mockCollection.RaiseCollectionChanged();
        }

        private class MockNotifyPropertyChanged : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class MockNotifyCollectionChanged : INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public void RaiseCollectionChanged()
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}
