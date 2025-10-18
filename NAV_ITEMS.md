# Navigation Items & Links

## Main Navigation
- **Home** → `/`
- **Counter** → `/counter`  
- **Weather** → `/weather`

## Invoice Management  
- **Invoice List** → `/InvoiceList`
- **Create Invoice** → `/CreateInvoice`
- **Update Invoice** → `/UpdateInvoice`

## Money Management
- **Add Money** → `/AddMoney`
- **Delete Money** → `/DeleteMoney`
- **Update Money** → `/UpdateMoney`

## Country Management
- **Country List** → `/ListCountry`
- **Add Country** → `/AddCountry`
- **Edit Country** → `/EditCountry`
- **Delete Country** → `/DeleteCountry`
- **Update Country** → `/UpdateCountry`

## Authentication (Conditional)
**Logged In:**
- **Profile** → `/Account/Manage`
- **Logout** → `Account/Logout` (POST)

**Not Logged In:**
- **Register** → `/Account/Register`
- **Login** → `/Account/Login`

## Account Management
- **Profile** → `/Account/Manage`
- **Email** → `/Account/Manage/Email`
- **Password** → `/Account/Manage/ChangePassword`
- **Two-factor Auth** → `/Account/Manage/TwoFactorAuthentication`
- **Personal Data** → `/Account/Manage/PersonalData`

## Issues Found
- Duplicate navbar brand in NavMenu.razor (lines 7 & 101)
- All page components exist and links are valid