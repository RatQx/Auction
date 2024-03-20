export function calculateTimeDifference(endTime: string): string {
  const date1 = new Date().getTime();
  const date2 = new Date(endTime).getTime();
  const diff = date2 - date1;

  if (diff < 0) {
    return 'Auction ended';
  }

  const millisecondsPerSecond = 1000;
  const millisecondsPerMinute = 60 * millisecondsPerSecond;
  const millisecondsPerHour = 60 * millisecondsPerMinute;
  const millisecondsPerDay = 24 * millisecondsPerHour;

  const days = Math.floor(diff / millisecondsPerDay);
  const hours = Math.floor((diff % millisecondsPerDay) / millisecondsPerHour);
  const minutes = Math.floor(
    (diff % millisecondsPerHour) / millisecondsPerMinute
  );
  const seconds = Math.floor(
    (diff % millisecondsPerMinute) / millisecondsPerSecond
  );

  return `${days} days, ${hours} hours, ${minutes} minutes, ${seconds} seconds`;
}
