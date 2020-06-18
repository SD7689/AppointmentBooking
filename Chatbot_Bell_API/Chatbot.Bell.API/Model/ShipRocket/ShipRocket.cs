using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model.ShipRocket
{
    public class ShipRocket
    {
    }
    #region Request
    public class AuthKeyRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class ReqCouriersServiceability
    {
        public int pickup_postcode { get; set; }
        public int delivery_postcode { get; set; }
        public int cod { get; set; } = 0;///1 for Cash on Delivery and 0 for Prepaid orders.
        public double weight { get; set; } // The weight of shipment in kgs.
                                           //public int is_return { get; set; } // Whether the order is return order or not. 1 in case of Yes and 0 for No
    }
    public class ReqCouriersServiceabilitywithAWB
    {
        public int pickup_postcode { get; set; }
        public int delivery_postcode { get; set; }
        public int cod { get; set; } = 0;///1 for Cash on Delivery and 0 for Prepaid orders.
        public double weight { get; set; } // The weight of shipment in kgs.
        //public int is_return { get; set; } // Whether the order is return order or not. 1 in case of Yes and 0 for No
        public CreateCustomOrderRequest orderDetails { get; set; }
    }
    public class AWBRequest
    {
        [Required]
        public int shipment_id { get; set; }
        public int courier_id { get; set; }
    }

    public class GeneratePickupRequest
    {
        [JsonProperty("shipment_id")]
        public int[] ShipmentId { get; set; }
    }

    public class PrintManifestRequest
    {
        [JsonProperty("order_ids")]
        public int[] OrderIds { get; set; }

    }
    public class PrintInvoiceRequest
    {
        [JsonProperty("ids")]
        public int[] Ids { get; set; }

    }
    public class GetTrackingRequest
    {
        public string awb_no { get; set; }
    }
    #endregion
    public class APIAuthResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("company_id")]
        public long CompanyId { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
    public class ServiceAvailablilityResponse
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("covid_zones")]
        public CovidZones CovidZones { get; set; }

        [JsonProperty("is_latlong")]
        public long IsLatlong { get; set; }

        [JsonProperty("seller_address")]
        public object[] SellerAddress { get; set; }

    }
    public partial class CovidZones
    {
        [JsonProperty("pickup_zone")]
        public long PickupZone { get; set; }

        [JsonProperty("delivery_zone")]
        public long DeliveryZone { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("is_recommendation_enabled")]
        public long IsRecommendationEnabled { get; set; }

        [JsonProperty("recommended_by")]
        public RecommendedBy RecommendedBy { get; set; }

        [JsonProperty("child_courier_id")]
        public object ChildCourierId { get; set; }

        [JsonProperty("recommended_courier_company_id")]
        public long RecommendedCourierCompanyId { get; set; }

        [JsonProperty("shiprocket_recommended_courier_id")]
        public long ShiprocketRecommendedCourierId { get; set; }

        [JsonProperty("available_courier_companies")]
        public AvailableCourierCompany[] AvailableCourierCompanies { get; set; }
    }
    public class RecommendedBy
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class AvailableCourierCompany
    {
        [JsonProperty("base_courier_id")]
        public object BaseCourierId { get; set; }

        [JsonProperty("courier_type")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public int CourierType { get; set; }

        [JsonProperty("courier_company_id")]
        public int CourierCompanyId { get; set; }

        [JsonProperty("courier_name")]
        public string CourierName { get; set; }

        [JsonProperty("is_rto_address_available")]
        public bool IsRtoAddressAvailable { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("is_custom_rate")]
        public long IsCustomRate { get; set; }

        [JsonProperty("cod_multiplier")]
        public long CodMultiplier { get; set; }

        [JsonProperty("cod_charges")]
        public long CodCharges { get; set; }

        [JsonProperty("freight_charge")]
        public double FreightCharge { get; set; }

        [JsonProperty("rto_charges")]
        public double RtoCharges { get; set; }

        [JsonProperty("is_surface")]
        public bool IsSurface { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("rto_performance")]
        public double RtoPerformance { get; set; }

        [JsonProperty("pickup_performance")]
        public double PickupPerformance { get; set; }

        [JsonProperty("delivery_performance")]
        public double DeliveryPerformance { get; set; }

        [JsonProperty("cod")]
        public long Cod { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("mode")]
        public long Mode { get; set; }

        [JsonProperty("min_weight")]
        public double MinWeight { get; set; }

        [JsonProperty("is_international")]
        public long IsInternational { get; set; }

        [JsonProperty("entry_tax")]
        public long EntryTax { get; set; }

        [JsonProperty("pickup_availability")]
        public long PickupAvailability { get; set; }

        [JsonProperty("seconds_left_for_pickup")]
        public long SecondsLeftForPickup { get; set; }

        [JsonProperty("etd_hours")]
        public long EtdHours { get; set; }

        [JsonProperty("etd")]
        public string Etd { get; set; }

        [JsonProperty("estimated_delivery_days")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public long EstimatedDeliveryDays { get; set; }

        [JsonProperty("tracking_performance")]
        public double TrackingPerformance { get; set; }

        [JsonProperty("weight_cases")]
        public double WeightCases { get; set; }

        [JsonProperty("realtime_tracking")]
        public string RealtimeTracking { get; set; }

        [JsonProperty("delivery_boy_contact")]
        public string DeliveryBoyContact { get; set; }

        [JsonProperty("pod_available")]
        public string PodAvailable { get; set; }

        [JsonProperty("call_before_delivery")]
        public CallBeforeDelivery CallBeforeDelivery { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }

        [JsonProperty("cost")]
        public string Cost { get; set; }

        [JsonProperty("edd")]
        public string Edd { get; set; }

        [JsonProperty("base_weight")]
        public string BaseWeight { get; set; }

        [JsonProperty("pickup_priority")]
        public string PickupPriority { get; set; }

        [JsonProperty("odablock")]
        public bool Odablock { get; set; }
    }
    public enum CallBeforeDelivery { Available, NotAvailable };

    public enum Etd { Jun112020, Jun122020, Jun142020 };

    public enum PodAvailable { Instant, OnRequest };

    public enum RealtimeTracking { RealTime };
    public partial class AWBAPIResponse
    {
        [JsonProperty("awb_assign_status")]
        public long AwbAssignStatus { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("courier_company_id")]
        public int CourierCompanyId { get; set; }

        [JsonProperty("awb_code")]
        public string AwbCode { get; set; }

        [JsonProperty("cod")]
        public int Cod { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("shipment_id")]
        public long ShipmentId { get; set; }

        [JsonProperty("awb_code_status")]
        public long AwbCodeStatus { get; set; }

        [JsonProperty("assigned_date_time")]
        public AssignedDateTime AssignedDateTime { get; set; }

        [JsonProperty("applied_weight")]
        public double AppliedWeight { get; set; }

        [JsonProperty("company_id")]
        public long CompanyId { get; set; }

        [JsonProperty("courier_name")]
        public string CourierName { get; set; }

        [JsonProperty("child_courier_name")]
        public object ChildCourierName { get; set; }

        [JsonProperty("routing_code")]
        public string RoutingCode { get; set; }

        [JsonProperty("rto_routing_code")]
        public string RtoRoutingCode { get; set; }
    }

    public partial class AssignedDateTime
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("timezone_type")]
        public int TimezoneType { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }
    }


    public class GeneratePickupResponse
    {
        [JsonProperty("pickup_status")]
        public int PickupStatus { get; set; }

        [JsonProperty("response")]
        public ResponsePickup Response { get; set; }
    }

    public partial class ResponsePickup
    {
        [JsonProperty("pickup_token_number")]
        public string PickupTokenNumber { get; set; }

        [JsonProperty("pickup_scheduled_date")]
        public DateTimeOffset PickupScheduledDate { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("pickup_generated_date")]
        public PickupGeneratedDate PickupGeneratedDate { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }

    public class PickupGeneratedDate
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("timezone_type")]
        public long TimezoneType { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }
    }
    public class GenerateManifestResponse
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("manifest_url")]
        public Uri ManifestUrl { get; set; }


    }
    public class PrintManifestResponse
    {
        [JsonProperty("manifest_url")]
        public string ManifestUrl { get; set; }
    }

    public class GenerateLabelResponse
    {
        [JsonProperty("label_created")]
        public int label_created { get; set; }

        [JsonProperty("label_url")]
        public string label_url { get; set; }

        [JsonProperty("response")]
        public string response { get; set; }

        [JsonProperty("not_created")]
        public string[] not_created { get; set; } //assumed data type to be string
    }
    public class PrintInvoiceResponse
    {
        [JsonProperty("is_invoice_created")]
        public bool is_invoice_created { get; set; }

        [JsonProperty("invoice_url")]
        public string invoice_url { get; set; }

        [JsonProperty("not_created")]
        public string[] not_created { get; set; } //assumed data type to be string}
    }

    public class GetTrackingResponse
    {
        [JsonProperty("tracking_data")]
        public TrackingData tracking_data { get; set; }
    }

    public class ShipmentTrack
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("awb_code")]
        public string awb_code { get; set; }

        [JsonProperty("courier_company_id")]
        public int courier_company_id { get; set; }

        [JsonProperty("shipment_id")]
        public string shipment_id { get; set; }

        [JsonProperty("order_id")]
        public int order_id { get; set; }

        [JsonProperty("pickup_date")]
        public string pickup_date { get; set; }

        [JsonProperty("delivered_date")]
        public string delivered_date { get; set; }

        [JsonProperty("weight")]
        public string weight { get; set; }

        [JsonProperty("packages")]
        public int packages { get; set; }

        [JsonProperty("current_status")]
        public string current_status { get; set; }

        [JsonProperty("delivered_to")]
        public string delivered_to { get; set; }

        [JsonProperty("destination")]
        public string destination { get; set; }

        [JsonProperty("consignee_name")]
        public string consignee_name { get; set; }

        [JsonProperty("origin")]
        public string origin { get; set; }

        [JsonProperty("courier_agent_details")]
        public string courier_agent_details { get; set; }
    }

    public class ShipmentTrackActivity
    {
        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("activity")]
        public string activity { get; set; }

        [JsonProperty("location")]
        public string location { get; set; }
    }

    public class TrackingData
    {
        [JsonProperty("track_status")]
        public int track_status { get; set; }

        [JsonProperty("shipment_status")]
        public int shipment_status { get; set; }

        [JsonProperty("shipment_track")]
        public ShipmentTrack[] shipment_track { get; set; }

        [JsonProperty("shipment_track_activities")]
        public ShipmentTrackActivity[] shipment_track_activities { get; set; }

        [JsonProperty("track_url")]
        public string track_url { get; set; }

        [JsonProperty("edd")]
        public string edd { get; set; }
    }




    #region Shopping Bag classes
    public class CreateCustomOrderRequest
    {
        [Required]
        [JsonProperty("order_id")]
        public string order_id { get; set; }

        [Required]
        [JsonProperty("order_date")]
        public string order_date { get; set; }

        [Required]
        [JsonProperty("pickup_location")]
        public string pickup_location { get; set; }

        [JsonProperty("channel_id")]
        public string channel_id { get; set; } = string.Empty;

        [JsonProperty("comment")]
        public string comment { get; set; } = string.Empty;

        [JsonProperty("reseller_name")]
        public string reseller_name { get; set; } = string.Empty;

        [JsonProperty("company_name")]
        public string company_name { get; set; } = string.Empty;

        [Required]
        [JsonProperty("billing_customer_name")]
        public string billing_customer_name { get; set; }

        [JsonProperty("billing_last_name")]
        public string billing_last_name { get; set; } = string.Empty;

        [JsonProperty("billing_address")]
        public string billing_address { get; set; } = string.Empty;

        [JsonProperty("billing_address_2")]
        public string billing_address_2 { get; set; } = string.Empty;

        [JsonProperty("billing_isd_code")]
        public string billing_isd_code { get; set; } = string.Empty;

        [Required]
        [JsonProperty("billing_city")]
        public string billing_city { get; set; }

        [Required]
        [JsonProperty("billing_pincode")]
        public string billing_pincode { get; set; }

        [Required]
        [JsonProperty("billing_state")]
        public string billing_state { get; set; }

        [Required]
        [JsonProperty("billing_country")]
        public string billing_country { get; set; }

        [Required]
        [JsonProperty("billing_email")]
        public string billing_email { get; set; }

        [Required]
        [JsonProperty("billing_phone")]
        public string billing_phone { get; set; }

        [JsonProperty("billing_alternate_phone")]
        public string billing_alternate_phone { get; set; } = string.Empty;

        [JsonProperty("shipping_is_billing")]
        public bool shipping_is_billing { get; set; }

        [Required]
        [JsonProperty("shipping_customer_name")]
        public string shipping_customer_name { get; set; }

        [JsonProperty("shipping_last_name")]
        public string shipping_last_name { get; set; }

        [Required]
        [JsonProperty("shipping_address")]
        public string shipping_address { get; set; }

        [JsonProperty("shipping_address_2")]
        public string shipping_address_2 { get; set; }

        [Required]
        [JsonProperty("shipping_city")]
        public string shipping_city { get; set; }

        [Required]
        [JsonProperty("shipping_pincode")]
        public string shipping_pincode { get; set; }

        [Required]
        [JsonProperty("shipping_country")]
        public string shipping_country { get; set; }

        [Required]
        [JsonProperty("shipping_state")]
        public string shipping_state { get; set; }

        [Required]
        [JsonProperty("shipping_email")]
        public string shipping_email { get; set; }

        [Required]
        [JsonProperty("shipping_phone")]
        public string shipping_phone { get; set; }

        [Required]
        [JsonProperty("order_items")]
        public OrderItem[] order_items { get; set; }

        [Required]
        [JsonProperty("payment_method")]
        public string payment_method { get; set; }

        [JsonProperty("shipping_charges")]
        public int shipping_charges { get; set; }

        [JsonProperty("giftwrap_charges")]
        public int giftwrap_charges { get; set; }

        [JsonProperty("transaction_charges")]
        public int transaction_charges { get; set; }

        [JsonProperty("total_discount")]
        public int total_discount { get; set; }

        [Required]
        [JsonProperty("sub_total")]
        public int sub_total { get; set; }

        [Required]
        [JsonProperty("length")]
        public int length { get; set; }

        [Required]
        [JsonProperty("breadth")]
        public int breadth { get; set; }

        [Required]
        [JsonProperty("height")]
        public int height { get; set; }

        [Required]
        [JsonProperty("weight")]
        public decimal weight { get; set; }

        [JsonProperty("ewaybill_no")]
        public string ewaybill_no { get; set; }

        [JsonProperty("customer_gstin")]
        public string customer_gstin { get; set; }
    }

    public class OrderItem
    {
        [Required]
        [JsonProperty("name")]
        public string name { get; set; }

        [Required]
        [JsonProperty("sku")]
        public string sku { get; set; }

        [Required]
        [JsonProperty("units")]
        public int units { get; set; }

        [Required]
        [JsonProperty("selling_price")]
        public string selling_price { get; set; }

        [JsonProperty("discount")]
        public int discount { get; set; }

        [JsonProperty("tax")]
        public int tax { get; set; }

        [JsonProperty("hsn")]
        public int hsn { get; set; }
    }

    public class CreateCustomOrderResponse
    {
        [JsonProperty("order_id")]
        public int order_id { get; set; }

        [JsonProperty("shipment_id")]
        public int shipment_id { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("status_code")]
        public int status_code { get; set; }

        [JsonProperty("onboarding_completed_now")]
        public int onboarding_completed_now { get; set; }

        [JsonProperty("awb_code")]
        public string awb_code { get; set; }

        [JsonProperty("courier_company_id")]
        public string courier_company_id { get; set; }

        [JsonProperty("courier_name")]
        public string courier_name { get; set; }
    }

    public class SBAWBResponse
    {
        public string awb_code { get; set; }
        public string order_id { get; set; }
        public string shipment_id { get; set; }
        public string courier_company_id { get; set; }
        public string courier_name { get; set; }
        public double rate { get; set; }
        public string is_custom_rate { get; set; }
        public string cod_multiplier { get; set; }
        public string cod_charges { get; set; }
        public string freight_charge { get; set; }
        public string rto_charges { get; set; }
        public string min_weight { get; set; }
        public string etd_hours { get; set; }
        public string etd { get; set; }
        public string estimated_delivery_days { get; set; }
    }


    public class GenericAPIResponse
    {
        public string message { get; set; }

        public int status_code { get; set; }
    }
    public class GenericmanifestsAPIResponse
    {
        public string message { get; set; }

        public int status_code { get; set; }

        public IList<int> already_manifested_shipment_ids { get; set; }
    }
    #endregion
}
