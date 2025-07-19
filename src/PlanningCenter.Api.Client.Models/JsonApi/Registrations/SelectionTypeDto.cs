using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

public class SelectionTypeDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "SelectionType";

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("attributes")]
    public SelectionTypeAttributes Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks Links { get; set; }
}

public class SelectionTypeAttributes
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("publicly_available")]
    public bool PubliclyAvailable { get; set; }

    [JsonPropertyName("price_cents")]
    public int PriceCents { get; set; }

    [JsonPropertyName("price_currency")]
    public string PriceCurrency { get; set; }

    [JsonPropertyName("price_currency_symbol")]
    public string PriceCurrencySymbol { get; set; }

    [JsonPropertyName("price_formatted")]
    public string PriceFormatted { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
