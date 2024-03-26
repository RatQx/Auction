export interface Payment {
  Id: number;
  Payment_Id: string;
  Payment_Time: Date;
  Payment_Amount: number;
  Payment_Successful: Boolean;
  Address_Line1: string;
  Address_Line2: string;
  Country: string;
  Postal_Code: string;
  Buyer_Id: string;
  Auction_Id: string;
  Buyer_Email: string;
  Auction_Owner_Email: string;
  Payment_Currency: string;
}
