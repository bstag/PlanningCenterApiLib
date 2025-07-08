# Giving Models

This folder contains C# models specific to the Planning Center Giving API.

## Batch

A `Batch` is a collection of `Donation`s.

### Attributes

*   `Id`: The unique identifier for a batch.
*   `CreatedAt`: The date and time at which a batch was created.
*   `UpdatedAt`: The date and time at which a batch was last updated.
*   `CommittedAt`: The date and time at which a batch was committed.
*   `Description`: The description for a batch.
*   `DonationsCount`: The number of donations in the batch.
*   `TotalCents`: The total amount of all donations in the batch in cents.
*   `TotalCurrency`: The currency of `total_cents`.
*   `Status`: The status of the batch.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/batches`

## BatchGroup

A `BatchGroup` is a collection of `Batch`es.

### Attributes

*   `Id`: The unique identifier for a batch group.
*   `CreatedAt`: The date and time at which a batch group was created.
*   `UpdatedAt`: The date and time at which a batch group was last updated.
*   `Description`: The description for a batch group.
*   `Committed`: Indicates whether the batch group is committed.
*   `TotalCents`: The total amount of all donations in the batch group in cents.
*   `TotalCurrency`: The currency of `total_cents`.
*   `Status`: The status of the batch group.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/batch_groups`

## Campus

A `Campus` that has been added to your `Organization`.

### Attributes

*   `Id`: The unique identifier for a campus.
*   `Name`: The name for a campus.
*   `Address`: The address for a campus (JSON object).

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/campuses`

## Designation

A `Designation` conveys how much of a `Donation` goes to a particular `Fund`.

### Attributes

*   `Id`: The unique identifier for a designation.
*   `AmountCents`: The number of cents being donated to a designation's associated fund.
*   `AmountCurrency`: The currency of `amount_cents`.
*   `FeeCents`: The fee amount distributed to a donation's designation.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/donations/{donation_id}/designations`

## DesignationRefund

A record that links a `Refund` with a `Designation`.

### Attributes

*   `Id`: The unique identifier for a designation refund.
*   `AmountCents`: The number of cents being refunded.
*   `AmountCurrency`: The currency of `amount_cents`.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/donations/{donation_id}/refund/designation_refunds`

## Donation

A `Donation` record corresponds to a gift given to an `Organization` at a particular point in time.

### Attributes

*   `Id`: The unique identifier for a donation.
*   `CreatedAt`: The date and time at which a donation was created.
*   `UpdatedAt`: The date and time at which a donation was last updated.
*   `PaymentMethodSub`: The payment method subtype.
*   `PaymentLast4`: The last 4 digits of the payment method.
*   `PaymentBrand`: The brand of the payment method.
*   `PaymentCheckNumber`: The check number for check payments.
*   `PaymentCheckDatedAt`: The date the check was dated.
*   `FeeCents`: The fee amount in cents.
*   `PaymentMethod`: The payment method used.
*   `ReceivedAt`: The date and time the donation was received.
*   `AmountCents`: The amount of the donation in cents.
*   `PaymentStatus`: The status of the payment.
*   `CompletedAt`: The date and time the donation was completed.
*   `FeeCovered`: Indicates if the fee was covered.
*   `AmountCurrency`: The currency of the amount.
*   `FeeCurrency`: The currency of the fee.
*   `Refunded`: Indicates if the donation was refunded.
*   `Refundable`: Indicates if the donation is refundable.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/donations`

## Fund

A `Fund` is a way of tracking the intent of `Donation`.

### Attributes

*   `Id`: The unique identifier for a fund.
*   `CreatedAt`: The date and time at which a fund was created.
*   `UpdatedAt`: The date and time at which a fund was last updated.
*   `Name`: The name for a fund.
*   `LedgerCode`: The ledger code for a fund.
*   `Description`: The description for a fund.
*   `Visibility`: The visibility of the fund.
*   `Default`: Indicates if this is the default fund.
*   `Color`: The color associated with the fund.
*   `Deletable`: Indicates if the fund is deletable.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/funds`

## InKindDonation

An `InKindDonation` record represents a non-cash gift given to an `Organization` at a specific time.

### Attributes

*   `Id`: The unique identifier for an in-kind donation.
*   `CreatedAt`: The date and time at which an in-kind donation was created.
*   `UpdatedAt`: The date and time at which an in-kind donation was last updated.
*   `Description`: The description of the in-kind donation.
*   `ExchangeDetails`: Details about the exchange.
*   `FairMarketValueCents`: The fair market value in cents.
*   `ReceivedOn`: The date the in-kind donation was received.
*   `ValuationDetails`: Details about the valuation.
*   `AcknowledgmentLastSentAt`: The date and time the acknowledgment was last sent.
*   `FairMarketValueCurrency`: The currency of the fair market value.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/in_kind_donations`

## Label

A `Label` is a way for Admins to manage and categorize `Donation`s.

### Attributes

*   `Id`: The unique identifier for a label.
*   `Slug`: The label text itself.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/labels`

## Note

### Attributes

*   `Id`: The unique identifier for the note.
*   `Body`: The body of the note.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/donations/{donation_id}/note`

## Organization

The root level `Organization` record which serves as a link to `Donation`s, `People`, `Fund`s, etc.

### Attributes

*   `Id`: The unique identifier for an organization.
*   `Name`: The name for an organization.
*   `TimeZone`: The time zone for an organization.
*   `Text2GiveEnabled`: `true` if this organization is accepting Text2Give donations, `false` otherwise.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2`

## PaymentMethod

Stored `PaymentMethod` information (`card` or `bank_account`) used by donors to make online `Donation`s.

### Attributes

*   `Id`: The unique identifier for a payment method.
*   `CreatedAt`: The date and time at which a payment method was created.
*   `UpdatedAt`: The date and time at which a payment method was last updated.
*   `MethodType`: Determines whether or not the payment method is a card or bank account.
*   `MethodSubtype`: The subtype of the payment method.
*   `Last4`: The last 4 digits of the payment method.
*   `Brand`: The brand of the payment method.
*   `Expiration`: The expiration date of the payment method.
*   `Verified`: Indicates whether the payment method is verified.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/people/{person_id}/payment_methods`

## PaymentSource

A donation's `PaymentSource` refers to the platform it originated from.

### Attributes

*   `Id`: The unique identifier for a payment source.
*   `CreatedAt`: The date and time at which a payment source was created.
*   `UpdatedAt`: The date and time at which a payment source was last updated.
*   `Name`: The name of a payment source.
*   `PaymentSourceType`: The type of payment source.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/payment_sources`

## Person

A Planning Center `Person` record that has been added to Giving.

### Attributes

*   `Id`: The unique identifier for a person.
*   `Permissions`: The level of Giving access granted to a person.
*   `EmailAddresses`: An array of email addresses for a person.
*   `Addresses`: An array of addresses for a person.
*   `PhoneNumbers`: An array of phone numbers for a person.
*   `FirstName`: The person's first name.
*   `LastName`: The person's last name.
*   `DonorNumber`: The donor number for the person.
*   `FirstDonatedAt`: The date and time the person first donated.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/people`

## Pledge

A `Pledge` made by a `Person` toward a particular `PledgeCampaign`.

### Attributes

*   `Id`: The unique identifier for a pledge.
*   `CreatedAt`: The date and time at which a pledge was created.
*   `UpdatedAt`: The date and time at which a pledge was last updated.
*   `AmountCents`: The amount pledged.
*   `AmountCurrency`: The currency of the pledged amount.
*   `JointGiverAmountCents`: The amount pledged by the joint giver, if in a joint giving unit.
*   `DonatedTotalCents`: The total amount donated towards the pledge.
*   `JointGiverDonatedTotalCents`: The total amount donated by the joint giver towards the pledge.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/pledges`

## PledgeCampaign

A `PledgeCampaign` is a way to request and track long-terms commitments to a particular goal or project.

### Attributes

*   `Id`: The unique identifier for a pledge campaign.
*   `CreatedAt`: The date and time at which a pledge campaign was created.
*   `UpdatedAt`: The date and time at which a pledge campaign was last updated.
*   `Name`: The name of the pledge campaign.
*   `Description`: The description of the pledge campaign.
*   `StartsAt`: The start date and time of the pledge campaign.
*   `EndsAt`: The end date and time of the pledge campaign.
*   `GoalCents`: The goal amount in cents.
*   `GoalCurrency`: The currency of the goal amount.
*   `ShowGoalInChurchCenter`: Indicates whether to show the goal in Church Center.
*   `ReceivedTotalFromPledgesCents`: The total amount received from pledges in cents.
*   `ReceivedTotalOutsideOfPledgesCents`: The total amount received outside of pledges in cents.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/pledge_campaigns`

## RecurringDonation

A `RecurringDonation` is represents a `Donation` that repeats on a set schedule (weekly, monthly, etc.).

### Attributes

*   `Id`: The unique identifier for a recurring donation.
*   `CreatedAt`: The date and time at which a recurring donation was created.
*   `UpdatedAt`: The date and time at which a recurring donation was last updated.
*   `ReleaseHoldAt`: The date when the hold on a recurring donation with a status of `temporary_hold` will be released.
*   `AmountCents`: The amount of the recurring donation in cents.
*   `Status`: The status of the recurring donation.
*   `LastDonationReceivedAt`: The date and time the last donation was received.
*   `NextOccurrence`: The date and time of the next occurrence.
*   `Schedule`: The schedule of the recurring donation (JSON object).
*   `AmountCurrency`: The currency of the amount.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/recurring_donations`

## RecurringDonationDesignation

Much like a `Designation`, A `RecurringDonationDesignation` conveys how much of a `RecurringDonation` goes to a particular `Fund`.

### Attributes

*   `Id`: The unique identifier for a recurring donation designation.
*   `AmountCents`: The number of cents that will be donated to a recurring donation designation's associated fund.
*   `AmountCurrency`: The currency of `amount_cents`.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/recurring_donations/{recurring_donation_id}/designations`

## Refund

A `Refund` record holds information pertaining to a refunded `Donation`.

### Attributes

*   `Id`: The unique identifier for a refund.
*   `CreatedAt`: The date and time at which a refund was created.
*   `UpdatedAt`: The date and time at which a refund was last updated.
*   `AmountCents`: The number of cents being refunded.
*   `AmountCurrency`: The currency of `amount_cents`.
*   `FeeCents`: The payment processing fee returned by Stripe, if any.
*   `RefundedAt`: The date and time the refund was processed.
*   `FeeCurrency`: The currency of the fee.

### Endpoints

*   **Giving API:** `https://api.planningcenteronline.com/giving/v2/donations/{donation_id}/refund`
