﻿using System;
using System.Collections.Generic;

namespace FlutterWave.Model
{
    public class FlutterWaveResponse
    {
        public int id { get; set; }
        public string account_number { get; set; }
        public string account_name { get; set; }
        public string bank_code { get; set; }
        public string full_name { get; set; }
        public DateTime created_at { get; set; }
        public string currency { get; set; }
        public string debit_currency { get; set; }
        public string processor_response { get; set; }
        public int? amount { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public object meta { get; set; }
        public string narration { get; set; }
        public string complete_message { get; set; }
        public int requires_approval { get; set; }
        public int is_approved { get; set; }
        public string bank_name { get; set; }
        public dynamic Fee { get; set; }
        public string Link { get; set; }
        public decimal? app_fee { get; set; }
        public double? amount_settled { get; set; }
        public string tx_ref { get; set; }
    }
    public abstract class BaseFlutterWave
    {
        public string status { get; set; }
        public string message { get; set; }
    }
    public class FlutterBaseResponse:BaseFlutterWave
    {
      
        public FlutterWaveResponse data { get; set; }
    }

    public class FlutterSearchResponse:BaseFlutterWave
    {

        public FlutterWaveResponse[] data { get; set; }
    }
    public class FLutterBalance : BaseFlutterWave
    {
        public List<FLutterBalanceData> data { get; set; }
    }
    public class FLutterBalanceData
    {
        public string currency { get; set; }
        public int available_balance { get; set; }
        public double ledger_balance { get; set; }
    }
    public class FlutterWaveBillResponse
    {
        public string phone_number { get; set; }
        public int amount { get; set; }
        public string network { get; set; }
        public string flw_ref { get; set; }
        public string reference { get; set; }
    }

    public class FlutterBaseBillResponse
    {
        public string status { get; set; }
        public string message { get; set; }

        public FlutterWaveBillResponse data { get; set; }
    }

    #region Flutterwave Response
    public class Card
    {
        public string first_6digits { get; set; }
        public string last_4digits { get; set; }
        public string issuer { get; set; }
        public string country { get; set; }
        public string type { get; set; }
        public string token { get; set; }
        public string expiry { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string tx_ref { get; set; }
        public string flw_ref { get; set; }
        public string device_fingerprint { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public double charged_amount { get; set; }
        public double app_fee { get; set; }
        public double merchant_fee { get; set; }
        public string processor_response { get; set; }
        public string auth_model { get; set; }
        public string ip { get; set; }
        public string narration { get; set; }
        public string status { get; set; }
        public string payment_type { get; set; }
        public DateTime created_at { get; set; }
        public int account_id { get; set; }
        public Card card { get; set; }
        public Meta meta { get; set; }
        public double amount_settled { get; set; }
        public Customer customer { get; set; }
    }

    public class Meta
    {
        public string __CheckoutInitAddress { get; set; }
    }

    public class FlutterwaveTransactionVerificationResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    #endregion
}