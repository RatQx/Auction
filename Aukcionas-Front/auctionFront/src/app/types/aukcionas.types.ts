export type RegisterRequest = {
  userName: string;
  email: string;
  password: string;
};
export type User = {
  id?: string;
  username: string;
  auction_won: string[];
  email: string;
};
export type LoginRequest = {
  userName: string;
  password: string;
};
export type LoginResponse = {
  accessToken: string;
};
