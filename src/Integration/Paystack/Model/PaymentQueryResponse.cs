using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paystack.Model
{
    public class Authorization
    {
    }

    public class Customer
    {
        public int Id { get; set; }
        public object First_Name { get; set; }
        public object Last_Name { get; set; }
        public string Email { get; set; }
        public string Customer_Code { get; set; }
        public object Phone { get; set; }
        public object MetaData { get; set; }
        public string Risk_Action { get; set; }
        public object International_Format_Phone { get; set; }
    }

    public class PaymentDetails
    {
        public long Id { get; set; }
        public string Domain { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public object Receipt_Number { get; set; }
        public decimal Amount { get; set; }
        public object Message { get; set; }
        public string Gateway_Response { get; set; }
        public object Paid_At { get; set; }
        public DateTime Created_At { get; set; }
        public string Channel { get; set; }
        public string Currency { get; set; }
        public string Ip_Address { get; set; }
        public string MetaData { get; set; }
        public object Log { get; set; }
        public decimal Fees { get; set; }
        public object Fees_Split { get; set; }
        public Authorization Authorization { get; set; }
        public Customer Customer { get; set; }
        public object Plan { get; set; }
        public Split Split { get; set; }
        public object Order_Id { get; set; }
        public object PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Requested_Amount { get; set; }
        public object Pos_Transaction_Data { get; set; }
        public object Source { get; set; }
        public object Fees_BreakDown { get; set; }
        public DateTime Transaction_Date { get; set; }
        public PlanObject Plan_Object { get; set; }
        public SubAccount SubAccount { get; set; }
    }

    public class PlanObject
    {
    }

    public class PaymentQueryResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public PaymentDetails Data { get; set; }
    }

    public class Split
    {
    }

    public class SubAccount
    {
    }


}
