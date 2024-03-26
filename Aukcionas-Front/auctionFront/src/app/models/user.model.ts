export interface User {
  userName: string;
  email: string;
  name: string;
  surname: string;
  phoneNumber: string;
  auctions_Won: any[];
  liked_Auctions: any[];
  can_Bid: boolean;
  paypal: boolean;
  bank: boolean;
  paypal_Email: string;
  account_Holder_Name: string;
  account_Number: string;
  bank_Name: string;
  bic_Swift_Code: string;
}
