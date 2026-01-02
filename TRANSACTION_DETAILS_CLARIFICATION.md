# Transaction Details (معلومات معاملہ) - Data Structure Clarification

## ✅ CURRENT STRUCTURE (CORRECT)

### PropertyDetails Table
**Purpose:** Property information and summary
**Contains:**
- Property characteristics (area, rooms, floors)
- Property type
- Property address
- Property boundaries
- Document information
- Status and approval tracking
- **Summary fields:** Price, RoyaltyAmount, TransactionTypeId (for backward compatibility)

### BuyerDetails Table ✅
**Purpose:** Detailed transaction information per buyer (معلومات معاملہ)
**Contains:**
- Buyer personal information
- **Price** - Individual buyer's price
- **PriceText** - Price in text
- **RoyaltyAmount** - Individual royalty amount
- **HalfPrice** - Half price if applicable
- **SharePercentage** - Buyer's ownership percentage
- **ShareAmount** - Buyer's ownership amount
- **TransactionType** - Type of transaction (Purchase, Rent, etc.)
- **TransactionTypeDescription** - Custom description
- **RentStartDate, RentEndDate** - For lease transactions

### SellerDetails Table ✅
**Purpose:** Seller information with share tracking
**Contains:**
- Seller personal information
- **SharePercentage** - Seller's ownership percentage
- **ShareAmount** - Seller's ownership amount

---

## 📋 WHY THIS STRUCTURE?

### Multiple Buyers/Sellers Support
- Each buyer can have different price/share
- Each seller can have different share
- Supports complex transactions

### Example Transaction:
```
Property: House, 200 sqm

Seller 1: Ahmad (50% share)
Seller 2: Hassan (50% share)

Buyer 1: Ali (60% share, 60,000 AFN)
Buyer 2: Karim (40% share, 40,000 AFN)

Total: 100,000 AFN
```

---

## ✅ CORRECT DATA FLOW

### Step 1: Property Details
User enters:
- Property type, area, rooms
- Property address
- Property boundaries
- Document details
- Status

### Step 2: Seller Details (معلومات فروشنده)
User enters for each seller:
- Personal information
- Share percentage/amount

### Step 3: Buyer Details (معلومات معاملہ) ✅
User enters for each buyer:
- Personal information
- **Price** (transaction amount)
- **Share percentage/amount**
- **Transaction type**
- **Rent dates** (if applicable)

---

## 🎯 CONCLUSION

**The current structure is CORRECT.**

- ✅ Transaction details (معلومات معاملہ) are in BuyerDetails table
- ✅ Each buyer has individual price and share
- ✅ PropertyDetails has summary fields for backward compatibility
- ✅ No changes needed

**Note:** The PropertyDetails.Price, RoyaltyAmount, and TransactionTypeId fields are kept for:
1. Backward compatibility with existing code
2. Quick summary/total calculations
3. Reporting purposes

The detailed, per-buyer transaction information is correctly stored in BuyerDetails table.
