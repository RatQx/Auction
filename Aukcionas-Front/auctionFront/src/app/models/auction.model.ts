export class Auction {
  id?: number;
  name?: string;
  country?: string;
  city?: string;
  username?: string;
  starting_price?: number;
  bid_ammount?: number;
  min_buy_price?: number;
  auction_start_time?: Date;
  auction_end_time?: Date;
  auction_time?: Date;
  auction_ended?: boolean;
  auction_won?: boolean;
  auction_winner?: string;
  auction_biders_list?: string[];
  bidding_amount_history?: number[];
  bidding_times_history?: Date[];
  buy_now_price?: number;
  category?: string;
  description?: string;
  item_build_year?: number;
  item_mass?: number;
  condition?: string;
  material?: string;
  auction_likes?: number;
}

export type GetAuctionReq = { name?: string };
