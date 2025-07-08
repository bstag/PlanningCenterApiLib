# People Models

This folder contains C# models specific to the Planning Center People API.

## Person

Represents a single member/user of the application.

### Attributes

*   `Id`: The unique identifier for the person.
*   `Avatar`: File UUID for the person's avatar.
*   `FirstName`: The person's first name.
*   `LastName`: The person's last name.
*   `DemographicAvatarUrl`: URL for a demographic avatar.
*   `Name`: The person's full name.
*   `Status`: The person's status.
*   `RemoteId`: Remote ID for the person.
*   `AccountingAdministrator`: Indicates if the person is an accounting administrator.
*   `Anniversary`: The person's anniversary date.
*   `Birthdate`: The person's birthdate.
*   `Child`: Indicates if the person is a child.
*   `GivenName`: The person's given name.
*   `Grade`: The person's grade.
*   `GraduationYear`: The person's graduation year.
*   `MiddleName`: The person's middle name.
*   `Nickname`: The person's nickname.
*   `PeoplePermissions`: Permissions for the person in People.
*   `SiteAdministrator`: Indicates if the person is a site administrator.
*   `Gender`: The person's gender.
*   `InactivatedAt`: Date and time the person was inactivated.
*   `MedicalNotes`: Medical notes for the person.
*   `Membership`: The person's membership status.
*   `CreatedAt`: The date and time the person record was created.
*   `UpdatedAt`: The date and time the person record was last updated.
*   `CanCreateForms`: Indicates if the person can create forms.
*   `CanEmailLists`: Indicates if the person can email lists.
*   `DirectorySharedInfo`: Information shared in the directory (object).
*   `DirectoryStatus`: The person's directory status.
*   `PassedBackgroundCheck`: Indicates if the person passed a background check.
*   `ResourcePermissionFlags`: Resource permission flags (object).
*   `SchoolType`: The person's school type.
*   `LoginIdentifier`: The person's login identifier.
*   `MfaConfigured`: Indicates if MFA is configured for the person.
*   `StripeCustomerIdentifier`: Stripe customer identifier for the person.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people`

## Address

Represents a physical and/or mailing address for a person.

### Attributes

*   `Id`: The unique identifier for the address.
*   `City`: The city of the address.
*   `State`: The state of the address.
*   `Zip`: The zip code of the address.
*   `CountryCode`: The country code of the address.
*   `Location`: The location type of the address (e.g., 'Home', 'Work').
*   `Primary`: Indicates if this is the primary address.
*   `StreetLine1`: The first line of the street address.
*   `StreetLine2`: The second line of the street address.
*   `CreatedAt`: The date and time the address was created.
*   `UpdatedAt`: The date and time the address was last updated.
*   `CountryName`: The full country name of the address.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/addresses`

## App

An app is one of the handful of apps that Planning Center offers that organizations can subscribe to, e.g. Services, Registrations, etc.

### Attributes

*   `Id`: The unique identifier for the app.
*   `Name`: The name of the app.
*   `Url`: The URL of the app.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/apps`

## BackgroundCheck

Background Checks for a Person.

### Attributes

*   `Id`: The unique identifier for the background check.
*   `Current`: Indicates if the background check is current.
*   `Note`: A note for the background check.
*   `StatusUpdatedAt`: The date and time the status was last updated.
*   `ReportUrl`: The URL of the background check report.
*   `ExpiresOn`: The expiration date of the background check.
*   `Status`: The status of the background check (e.g., 'awaiting_applicant', 'complete_clear').
*   `CompletedAt`: The date and time the background check was completed.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/background_checks`

## BirthdayPeople

Returns upcoming birthdays for the organization.

### Attributes

*   `Id`: The unique identifier for the birthday people collection.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/birthday_people`

## Campus

A Campus is a location belonging to an Organization.

### Attributes

*   `Id`: The unique identifier for the campus.
*   `Latitude`: The latitude of the campus.
*   `Longitude`: The longitude of the campus.
*   `Description`: The description of the campus.
*   `Street`: The street address of the campus.
*   `City`: The city of the campus.
*   `State`: The state of the campus.
*   `Zip`: The zip code of the campus.
*   `Country`: The country of the campus.
*   `PhoneNumber`: The phone number of the campus.
*   `Website`: The website of the campus.
*   `TwentyFourHourTime`: Indicates if the campus uses 24-hour time.
*   `DateFormat`: The date format used by the campus.
*   `ChurchCenterEnabled`: Indicates if Church Center is enabled for the campus.
*   `ContactEmailAddress`: The contact email address for the campus.
*   `TimeZone`: The time zone of the campus.
*   `GeolocationSetManually`: Indicates if geolocation was set manually.
*   `TimeZoneRaw`: The raw time zone string.
*   `Name`: The name of the campus.
*   `CreatedAt`: The date and time the campus was created.
*   `UpdatedAt`: The date and time the campus was last updated.
*   `AvatarUrl`: The URL of the campus avatar.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/campuses`

## Carrier

### Attributes

*   `Id`: The unique identifier for the carrier.
*   `Value`: The value of the carrier.
*   `Name`: The name of the carrier.
*   `International`: Indicates if the carrier is international.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/carriers`

## Condition

A condition is an individual criterion used by a List Rule.

### Attributes

*   `Id`: The unique identifier for the condition.
*   `Application`: The application the condition belongs to.
*   `DefinitionClass`: The definition class of the condition.
*   `Comparison`: The comparison operator for the condition.
*   `Settings`: The settings for the condition.
*   `DefinitionIdentifier`: The identifier for the definition.
*   `Description`: A description of the condition.
*   `CreatedAt`: The date and time the condition was created.
*   `UpdatedAt`: The date and time the condition was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/lists/{list_id}/rules/{rule_id}/conditions`

## ConnectedPerson

A Connected Person is an account from a different organization linked to an account in this organization.

### Attributes

*   `Id`: The unique identifier for the connected person.
*   `GivenName`: The given name of the connected person.
*   `FirstName`: The first name of the connected person.
*   `Nickname`: The nickname of the connected person.
*   `MiddleName`: The middle name of the connected person.
*   `LastName`: The last name of the connected person.
*   `Gender`: The gender of the connected person.
*   `OrganizationName`: The name of the organization the connected person belongs to.
*   `OrganizationId`: The ID of the organization the connected person belongs to.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people/{person_id}/connected_people`

## CustomSender

A custom sender that can be used when sending emails.

### Attributes

*   `Id`: The unique identifier for the custom sender.
*   `Name`: The name of the custom sender.
*   `EmailAddress`: The email address of the custom sender.
*   `VerifiedAt`: The date and time the custom sender was verified.
*   `VerificationRequestedAt`: The date and time verification was requested.
*   `CreatedAt`: The date and time the custom sender was created.
*   `UpdatedAt`: The date and time the custom sender was last updated.
*   `Verified`: Indicates if the custom sender is verified.
*   `Expired`: Indicates if the custom sender is expired.
*   `VerificationStatus`: The verification status of the custom sender.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/custom_senders`

## Email

An email represents an email address and location.

### Attributes

*   `Id`: The unique identifier for the email.
*   `Address`: The email address.
*   `Location`: The location of the email (e.g., 'Home', 'Work').
*   `Primary`: Indicates if this is the primary email address.
*   `CreatedAt`: The date and time the email was created.
*   `UpdatedAt`: The date and time the email was last updated.
*   `Blocked`: Indicates if the email is blocked.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/emails`

## FieldDatum

A field datum is an individual piece of data for a custom field.

### Attributes

*   `Id`: The unique identifier for the field datum.
*   `Value`: The value of the field datum.
*   `File`: The file associated with the field datum.
*   `FileSize`: The size of the file.
*   `FileContentType`: The content type of the file.
*   `FileName`: The name of the file.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/field_data`

## FieldDefinition

A field definition represents a custom field -- its name, data type, etc.

### Attributes

*   `Id`: The unique identifier for the field definition.
*   `DataType`: The data type of the field.
*   `Name`: The name of the field.
*   `Sequence`: The sequence of the field.
*   `Slug`: The slug of the field.
*   `Config`: The configuration of the field.
*   `DeletedAt`: The date and time the field definition was deleted.
*   `TabId`: The ID of the tab the field belongs to.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/field_definitions`

## FieldOption

A field option represents an individual option for a custom field of type "select" or "checkboxes".

### Attributes

*   `Id`: The unique identifier for the field option.
*   `Value`: The value of the field option.
*   `Sequence`: The sequence of the field option.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/field_definitions/{field_definition_id}/field_options`

## Form

A custom form for people to fill out.

### Attributes

*   `Id`: The unique identifier for the form.
*   `Name`: The name of the form.
*   `Description`: The description of the form.
*   `Active`: Indicates if the form is active.
*   `ArchivedAt`: The date and time the form was archived.
*   `CreatedAt`: The date and time the form was created.
*   `UpdatedAt`: The date and time the form was last updated.
*   `DeletedAt`: The date and time the form was deleted.
*   `SubmissionCount`: The number of submissions for the form.
*   `PublicUrl`: The public URL of the form.
*   `RecentlyViewed`: Indicates if the form was recently viewed.
*   `Archived`: Indicates if the form is archived.
*   `SendSubmissionNotificationToSubmitter`: Indicates if submission notifications are sent to the submitter.
*   `LoginRequired`: Indicates if login is required to access the form.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/forms`

## FormCategory

A Form Category.

### Attributes

*   `Id`: The unique identifier for the form category.
*   `Name`: The name of the form category.
*   `CreatedAt`: The date and time the form category was created.
*   `UpdatedAt`: The date and time the form category was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/form_categories`

## FormField

A field in a custom form.

### Attributes

*   `Id`: The unique identifier for the form field.
*   `Label`: The label of the form field.
*   `Description`: The description of the form field.
*   `Required`: Indicates if the form field is required.
*   `Settings`: The settings for the form field.
*   `FieldType`: The type of the field (e.g., 'string', 'text', 'checkboxes').
*   `Sequence`: The sequence of the form field.
*   `CreatedAt`: The date and time the form field was created.
*   `UpdatedAt`: The date and time the form field was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/forms/{form_id}/fields`

## FormFieldOption

A field option on a custom form field.

### Attributes

*   `Id`: The unique identifier for the form field option.
*   `Label`: The label of the form field option.
*   `Sequence`: The sequence of the form field option.
*   `CreatedAt`: The date and time the form field option was created.
*   `UpdatedAt`: The date and time the form field option was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/forms/{form_id}/fields/{field_id}/options`

## FormSubmission

A form submission.

### Attributes

*   `Id`: The unique identifier for the form submission.
*   `Verified`: Indicates if the form submission is verified.
*   `RequiresVerification`: Indicates if the form submission requires verification.
*   `CreatedAt`: The date and time the form submission was created.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/forms/{form_id}/form_submissions`

## FormSubmissionValue

A form submission value.

### Attributes

*   `Id`: The unique identifier for the form submission value.
*   `DisplayValue`: The display value of the form submission value.
*   `Attachments`: A list of attachments for the form submission value.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/forms/{form_id}/form_submissions/{form_submission_id}/form_submission_values`

## Household

A household links people together and can have a primary contact.

### Attributes

*   `Id`: The unique identifier for the household.
*   `Name`: The name of the household.
*   `MemberCount`: The number of members in the household.
*   `PrimaryContactName`: The name of the primary contact for the household.
*   `CreatedAt`: The date and time the household was created.
*   `UpdatedAt`: The date and time the household was last updated.
*   `Avatar`: The avatar of the household.
*   `PrimaryContactId`: The ID of the primary contact for the household.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/households`

## HouseholdMembership

A household membership is the linking record between a household and a person.

### Attributes

*   `Id`: The unique identifier for the household membership.
*   `PersonName`: The name of the person in the household.
*   `Pending`: Indicates if the membership is pending.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/households/{household_id}/household_memberships`

## InactiveReason

An inactive reason is a small bit of text indicating why a member is no longer active.

### Attributes

*   `Id`: The unique identifier for the inactive reason.
*   `Value`: The value of the inactive reason.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/inactive_reasons`

## List

A list is a powerful tool for finding and grouping people together using any criteria imaginable.

### Attributes

*   `Id`: The unique identifier for the list.
*   `Name`: The name of the list.
*   `AutoRefresh`: Indicates if the list auto-refreshes.
*   `Status`: The status of the list.
*   `HasInactiveResults`: Indicates if the list has inactive results.
*   `IncludeInactive`: Indicates if inactive people are included.
*   `Returns`: What the list returns.
*   `ReturnOriginalIfNone`: Indicates if the original list is returned if none.
*   `Subset`: The subset of the list.
*   `AutomationsActive`: Indicates if automations are active for the list.
*   `AutomationsCount`: The number of automations for the list.
*   `PausedAutomationsCount`: The number of paused automations for the list.
*   `Description`: The description of the list.
*   `Invalid`: Indicates if the list is invalid.
*   `AutoRefreshFrequency`: The auto-refresh frequency of the list.
*   `NameOrDescription`: The name or description of the list.
*   `RecentlyViewed`: Indicates if the list was recently viewed.
*   `RefreshedAt`: The date and time the list was refreshed.
*   `Starred`: Indicates if the list is starred.
*   `TotalPeople`: The total number of people in the list.
*   `BatchCompletedAt`: The date and time the batch was completed.
*   `CreatedAt`: The date and time the list was created.
*   `UpdatedAt`: The date and time the list was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/lists`

## ListCategory

A List Category.

### Attributes

*   `Id`: The unique identifier for the list category.
*   `Name`: The name of the list category.
*   `CreatedAt`: The date and time the list category was created.
*   `UpdatedAt`: The date and time the list category was last updated.
*   `OrganizationId`: The ID of the organization the list category belongs to.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/list_categories`

## ListResult

A list result.

### Attributes

*   `Id`: The unique identifier for the list result.
*   `CreatedAt`: The date and time the list result was created.
*   `UpdatedAt`: The date and time the list result was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/lists/{list_id}/list_results`

## ListShare

A list share indicates who has access to edit a list.

### Attributes

*   `Id`: The unique identifier for the list share.
*   `Permission`: The permission level (e.g., 'view', 'manage').
*   `Group`: The group permission (e.g., 'No Access', 'Viewer').
*   `CreatedAt`: The date and time the list share was created.
*   `Name`: The name of the list share.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/lists/{list_id}/shares`

## ListStar

A starred list for a person indicates it is special in some way.

### Attributes

*   `Id`: The unique identifier for the list star.
*   `CreatedAt`: The date and time the list star was created.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/lists/{list_id}/star`

## MailchimpSyncStatus

The status of syncing a List with Mailchimp.

### Attributes

*   `Id`: The unique identifier for the Mailchimp sync status.
*   `Status`: The status of the sync.
*   `Error`: Any error message from the sync.
*   `Progress`: The progress of the sync (0-100).
*   `CompletedAt`: The date and time the sync was completed.
*   `SegmentId`: The Mailchimp segment ID.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/lists/{list_id}/mailchimp_sync_status`

## MaritalStatus

A marital status represents a member's current status, e.g. married, single, etc.

### Attributes

*   `Id`: The unique identifier for the marital status.
*   `Value`: The value of the marital status.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/marital_statuses`

## Message

A message is an individual email or sms text sent to a member.

### Attributes

*   `Id`: The unique identifier for the message.
*   `Kind`: The kind of message (e.g., 'email', 'sms').
*   `ToAddresses`: The recipient addresses.
*   `Subject`: The subject of the message.
*   `DeliveryStatus`: The delivery status of the message.
*   `RejectReason`: The reason for rejection.
*   `CreatedAt`: The date and time the message was created.
*   `SentAt`: The date and time the message was sent.
*   `BouncedAt`: The date and time the message bounced.
*   `RejectionNotificationSentAt`: The date and time rejection notification was sent.
*   `FromName`: The sender's name.
*   `FromAddress`: The sender's address.
*   `ReadAt`: The date and time the message was read.
*   `AppName`: The name of the app that sent the message.
*   `MessageType`: The type of message.
*   `File`: The file associated with the message.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/messages`

## MessageGroup

A message group represents one or more emails or text messages sent from one of the Planning Center apps.

### Attributes

*   `Id`: The unique identifier for the message group.
*   `Uuid`: The UUID of the message group.
*   `MessageType`: The type of message.
*   `FromAddress`: The sender's address.
*   `Subject`: The subject of the message group.
*   `MessageCount`: The number of messages in the group.
*   `SystemMessage`: Indicates if it's a system message.
*   `TransactionalMessage`: Indicates if it's a transactional message.
*   `ContainsUserGeneratedContent`: Indicates if it contains user-generated content.
*   `CreatedAt`: The date and time the message group was created.
*   `ReplyToName`: The reply-to name.
*   `ReplyToAddress`: The reply-to address.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/message_groups`

## NamePrefix

A name prefix is one of Mr., Mrs., etc.

### Attributes

*   `Id`: The unique identifier for the name prefix.
*   `Value`: The value of the name prefix.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/name_prefixes`

## NameSuffix

A name suffix is one of Sr., Jr., etc.

### Attributes

*   `Id`: The unique identifier for the name suffix.
*   `Value`: The value of the name suffix.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/name_suffixes`

## Note

A note is text with a category connected to a person’s profile.

### Attributes

*   `Id`: The unique identifier for the note.
*   `NoteContent`: The content of the note.
*   `CreatedAt`: The date and time the note was created.
*   `UpdatedAt`: The date and time the note was last updated.
*   `DisplayDate`: The display date of the note.
*   `NoteCategoryId`: The ID of the note category.
*   `OrganizationId`: The ID of the organization the note belongs to.
*   `PersonId`: The ID of the person the note is for.
*   `CreatedById`: The ID of the person who created the note.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/notes`

## NoteCategory

A Note Category.

### Attributes

*   `Id`: The unique identifier for the note category.
*   `Name`: The name of the note category.
*   `Locked`: Indicates if the note category is locked.
*   `CreatedAt`: The date and time the note category was created.
*   `UpdatedAt`: The date and time the note category was last updated.
*   `OrganizationId`: The ID of the organization the note category belongs to.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/note_categories`

## NoteCategoryShare

A note category share defines who can view notes in a category.

### Attributes

*   `Id`: The unique identifier for the note category share.
*   `Group`: The group permission (e.g., 'No Access', 'Viewer').
*   `Permission`: The permission level (e.g., 'view', 'view_create').
*   `PersonId`: The ID of the person the share is for.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/note_categories/{note_category_id}/shares`

## NoteCategorySubscription

A subscription for note categories.

### Attributes

*   `Id`: The unique identifier for the note category subscription.
*   `CreatedAt`: The date and time the note category subscription was created.
*   `UpdatedAt`: The date and time the note category subscription was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/note_category_subscriptions`

## Organization

The organization represents a single church. Every other resource is scoped to this record.

### Attributes

*   `Id`: The unique identifier for the organization.
*   `Name`: The name of the organization.
*   `CountryCode`: The country code of the organization.
*   `DateFormat`: The date format used by the organization.
*   `TimeZone`: The time zone of the organization.
*   `ContactWebsite`: The contact website of the organization.
*   `CreatedAt`: The date and time the organization was created.
*   `AvatarUrl`: The URL of the organization's avatar.
*   `ChurchCenterSubdomain`: The Church Center subdomain for the organization.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2`

## OrganizationStatistics

Returns statistics for the organization.

### Attributes

*   `Id`: The unique identifier for the organization statistics.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/organization_statistics`

## PeopleImport

A PeopleImport is a record of an ongoing or previous import from a CSV file.

### Attributes

*   `Id`: The unique identifier for the people import.
*   `Attribs`: The attributes of the import.
*   `Status`: The status of the import (e.g., 'matching', 'complete').
*   `CreatedAt`: The date and time the import was created.
*   `UpdatedAt`: The date and time the import was last updated.
*   `ProcessedAt`: The date and time the import was processed.
*   `UndoneAt`: The date and time the import was undone.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people_imports`

## PeopleImportConflict

A PeopleImportConflict is a record of change that will occur if the parent PeopleImport is completed.

### Attributes

*   `Id`: The unique identifier for the people import conflict.
*   `Kind`: The kind of conflict.
*   `Name`: The name of the conflict.
*   `Message`: The conflict message.
*   `Data`: The data related to the conflict.
*   `ConflictingChanges`: The conflicting changes.
*   `Ignore`: Indicates if the conflict should be ignored.
*   `CreatedAt`: The date and time the conflict was created.
*   `UpdatedAt`: The date and time the conflict was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people_imports/{people_import_id}/conflicts`

## PeopleImportHistory

A PeopleImportHistory is a record of change that occurred when the parent PeopleImport was completed.

### Attributes

*   `Id`: The unique identifier for the people import history.
*   `Name`: The name of the history entry.
*   `CreatedAt`: The date and time the history entry was created.
*   `UpdatedAt`: The date and time the history entry was last updated.
*   `ConflictingChanges`: Conflicting changes (object).
*   `Kind`: The kind of history entry.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people_imports/{people_import_id}/histories`

## PersonApp

A Person App is the relationship between a Person and an App.

### Attributes

*   `Id`: The unique identifier for the person app.
*   `AllowPcoLogin`: Indicates if PCO login is allowed.
*   `PeoplePermissions`: Permissions for the person in People (e.g., 'no_access', 'viewer').

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people/{person_id}/person_apps`

## PersonMerger

A Person Merger is the history of profiles that were merged into other profiles.

### Attributes

*   `Id`: The unique identifier for the person merger.
*   `CreatedAt`: The date and time the merger was created.
*   `PersonToKeepId`: The ID of the person to keep.
*   `PersonToRemoveId`: The ID of the person to remove.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/person_mergers`

## PhoneNumber

A phone number represents a single telephone number and location.

### Attributes

*   `Id`: The unique identifier for the phone number.
*   `Number`: The phone number.
*   `Carrier`: The carrier of the phone number.
*   `Location`: The location of the phone number (e.g., 'Home', 'Work').
*   `Primary`: Indicates if this is the primary phone number.
*   `CreatedAt`: The date and time the phone number was created.
*   `UpdatedAt`: The date and time the phone number was last updated.
*   `E164`: The phone number in E.164 format.
*   `International`: The international format of the phone number.
*   `National`: The national format of the phone number.
*   `CountryCode`: The country code of the phone number.
*   `FormattedNumber`: The formatted phone number.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/phone_numbers`

## PlatformNotification

A Platform Notification is a suite-wide notification that shows at the top of each application's screen until dismissed by the user.

### Attributes

*   `Id`: The unique identifier for the platform notification.
*   `Html`: The HTML content of the notification.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people/{person_id}/platform_notifications`

## Report

A report is editable liquid syntax that provides a powerful tool for presenting your Lists however you want.

### Attributes

*   `Id`: The unique identifier for the report.
*   `Name`: The name of the report.
*   `Body`: The body content of the report.
*   `CreatedAt`: The date and time the report was created.
*   `UpdatedAt`: The date and time the report was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/reports`

## Rule

A rule belongs to a List and groups conditions together.

### Attributes

*   `Id`: The unique identifier for the rule.
*   `Subset`: The subset of the rule.
*   `CreatedAt`: The date and time the rule was created.
*   `UpdatedAt`: The date and time the rule was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/lists/{list_id}/rules`

## SchoolOption

A school option represents a school name, school type, grades, etc. and can be selected for a person.

### Attributes

*   `Id`: The unique identifier for the school option.
*   `Value`: The value of the school option.
*   `Sequence`: The sequence of the school option.
*   `BeginningGrade`: The beginning grade for the school option.
*   `EndingGrade`: The ending grade for the school option.
*   `SchoolTypes`: A list of school types.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/school_options`

## ServiceTime

A ServiceTime Resource.

### Attributes

*   `Id`: The unique identifier for the service time.
*   `StartTime`: The start time of the service.
*   `Day`: The day of the week for the service (e.g., 'sunday', 'monday').
*   `Description`: A description of the service time.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/campuses/{campus_id}/service_times`

## SocialProfile

A social profile represents a member's Twitter, Facebook, or other social media account.

### Attributes

*   `Id`: The unique identifier for the social profile.
*   `Site`: The social media site (e.g., 'Twitter', 'Facebook').
*   `Url`: The URL of the social profile.
*   `Verified`: Indicates if the social profile is verified.
*   `CreatedAt`: The date and time the social profile was created.
*   `UpdatedAt`: The date and time the social profile was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/social_profiles`

## SpamEmailAddress

An email address that is marked as spam.

### Attributes

*   `Id`: The unique identifier for the spam email address.
*   `Address`: The spam email address.
*   `CreatedAt`: The date and time the spam email address was created.
*   `UpdatedAt`: The date and time the spam email address was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/spam_email_addresses`

## Tab

A tab is a custom tab and groups like field definitions.

### Attributes

*   `Id`: The unique identifier for the tab.
*   `Name`: The name of the tab.
*   `Sequence`: The sequence of the tab.
*   `Slug`: The slug of the tab.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/tabs`

## Workflow

A Workflow.

### Attributes

*   `Id`: The unique identifier for the workflow.
*   `Name`: The name of the workflow.
*   `MyReadyCardCount`: The number of ready cards assigned to the current user.
*   `TotalReadyCardCount`: The total number of ready cards.
*   `CompletedCardCount`: The number of completed cards.
*   `TotalCardsCount`: The total number of cards.
*   `TotalReadyAndSnoozedCardCount`: The total number of ready and snoozed cards.
*   `TotalStepsCount`: The total number of steps.
*   `TotalUnassignedStepsCount`: The total number of unassigned steps.
*   `TotalUnassignedCardCount`: The total number of unassigned cards.
*   `TotalOverdueCardCount`: The total number of overdue cards.
*   `CreatedAt`: The date and time the workflow was created.
*   `UpdatedAt`: The date and time the workflow was last updated.
*   `DeletedAt`: The date and time the workflow was deleted.
*   `ArchivedAt`: The date and time the workflow was archived.
*   `CampusId`: The ID of the campus the workflow belongs to.
*   `WorkflowCategoryId`: The ID of the workflow category.
*   `MyOverdueCardCount`: The number of overdue cards assigned to the current user.
*   `MyDueSoonCardCount`: The number of due soon cards assigned to the current user.
*   `RecentlyViewed`: Indicates if the workflow was recently viewed.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/workflows`

## WorkflowCard

A Card.

### Attributes

*   `Id`: The unique identifier for the workflow card.
*   `SnoozeUntil`: The date and time until which the card is snoozed.
*   `Overdue`: Indicates if the card is overdue.
*   `Stage`: The stage of the card.
*   `CalculatedDueAtInDaysAgo`: The calculated due date in days ago.
*   `StickyAssignment`: Indicates if the assignment is sticky.
*   `CreatedAt`: The date and time the card was created.
*   `UpdatedAt`: The date and time the card was last updated.
*   `CompletedAt`: The date and time the card was completed.
*   `FlaggedForNotificationAt`: The date and time the card was flagged for notification.
*   `RemovedAt`: The date and time the card was removed.
*   `MovedToStepAt`: The date and time the card was moved to a step.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people/{person_id}/workflow_cards`

## WorkflowCardActivity

Workflow Card Activity is a record of an action performed on a card.

### Attributes

*   `Id`: The unique identifier for the workflow card activity.
*   `Comment`: The comment for the activity.
*   `Content`: The content of the activity.
*   `FormSubmissionUrl`: The URL of the form submission.
*   `AutomationUrl`: The URL of the automation.
*   `PersonAvatarUrl`: The avatar URL of the person.
*   `PersonName`: The name of the person.
*   `ReassignedToAvatarUrl`: The avatar URL of the reassigned person.
*   `ReassignedToName`: The name of the reassigned person.
*   `Subject`: The subject of the activity.
*   `Type`: The type of activity.
*   `ContentIsHtml`: Indicates if the content is HTML.
*   `CreatedAt`: The date and time the activity was created.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people/{person_id}/workflow_cards/{workflow_card_id}/activities`

## WorkflowCardNote

Workflow Note is a note that has been made on a Workflow Card.

### Attributes

*   `Id`: The unique identifier for the workflow card note.
*   `Note`: The content of the note.
*   `CreatedAt`: The date and time the note was created.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people/{person_id}/workflow_cards/{workflow_card_id}/notes`

## WorkflowCategory

A Workflow Category.

### Attributes

*   `Id`: The unique identifier for the workflow category.
*   `Name`: The name of the workflow category.
*   `CreatedAt`: The date and time the workflow category was created.
*   `UpdatedAt`: The date and time the workflow category was last updated.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/workflow_categories`

## WorkflowShare

A workflow share defines who can access a workflow.

### Attributes

*   `Id`: The unique identifier for the workflow share.
*   `Group`: The group permission (e.g., 'No Access', 'Viewer').
*   `Permission`: The permission level (e.g., 'view', 'manage_cards').
*   `PersonId`: The ID of the person the share is for.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/people/{person_id}/workflow_shares`

## WorkflowStep

A Step.

### Attributes

*   `Id`: The unique identifier for a workflow step.
*   `Sequence`: The sequence of the step.
*   `Name`: The name of the step.
*   `Description`: The description of the step.
*   `ExpectedResponseTimeInDays`: The expected response time in days.
*   `AutoSnoozeValue`: The value for auto snooze.
*   `AutoSnoozeInterval`: The interval for auto snooze.
*   `CreatedAt`: The date and time the step was created.
*   `UpdatedAt`: The date and time the step was last updated.
*   `AutoSnoozeDays`: The number of days for auto snooze.
*   `MyReadyCardCount`: The number of ready cards assigned to the current user for this step.
*   `TotalReadyCardCount`: The total number of ready cards for this step.
*   `DefaultAssigneeId`: The ID of the default assignee for this step.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/workflows/{workflow_id}/steps`

## WorkflowStepAssigneeSummary

The ready and snoozed count for an assignee & step.

### Attributes

*   `Id`: The unique identifier for the workflow step assignee summary.
*   `ReadyCount`: The number of ready cards.
*   `SnoozedCount`: The number of snoozed cards.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2/workflows/{workflow_id}/steps/{step_id}/assignee_summaries`
