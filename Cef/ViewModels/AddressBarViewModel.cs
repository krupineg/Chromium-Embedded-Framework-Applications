using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommonsLib;

namespace Cef
{
    public class AddressBarViewModel : ViewModel
    {
        private readonly ILogger _logger;
        private readonly QueryPredictor _predictor;
        private string _address;
        private ReadOnlyCollection<string> _prediction;
        private string _selectedPrediction;
        private bool _predict;

        public ReadOnlyCollection<string> Prediction
        {
            get { return _prediction; }
            set { SetProperty(ref _prediction, value); }
        }

        public string SelectedPrediction
        {
            get { return _selectedPrediction; }
            set
            {
                SetProperty(ref _selectedPrediction, value);
                Predict = false;
                Address = value;
                Predict = true;
            }
        }

        public bool Predict
        {
            get { return _predict; }
            set { SetProperty(ref _predict, value); }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value);
                Predict = _address != null && !_address.StartsWith("http");
                HandleAddressChangedAsync(_address);
            }
        }

        private void HandleAddressChangedAsync(string value)
        {
            if (Predict)
            {
                _predictor.Predict(value).ContinueWith(task =>
                {
                    try
                    {
                        var prediction = task.Result.Take(6).ToList();
                        Prediction = new ReadOnlyCollection<string>(prediction);
                        LogPrediction(value, prediction);
                    }
                    catch (Exception e)
                    {
                        _logger.Info("prediction error: " + e.Message);
                    }
                   
                });
            }
        }

        private void LogPrediction(string value, List<string> prediction)
        {
            var sb = new StringBuilder("predicted for ").Append(value).Append(":");
            foreach (var pred in prediction)
            {
                sb.Append(Environment.NewLine).Append(pred);
            }
            _logger.Info(sb.ToString());
        }

        public AddressBarViewModel(string startingAddress, ILogger logger)
        {
            _predictor = new QueryPredictor();
            _logger = logger;
            Address = startingAddress;
        }
    }
   

}