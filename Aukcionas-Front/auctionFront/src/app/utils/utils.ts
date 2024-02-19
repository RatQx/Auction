import { Params } from '@angular/router';

export function removeEmptyParams(params?: Params): Params {
  const nonEmptyParams: Params = {};
  if (params) {
    Object.entries(params).forEach(([key, value]) => {
      if (value !== '' && value !== null && value !== undefined) {
        nonEmptyParams[key] = value;
      }
    });
  }
  return nonEmptyParams;
}
export function objectToParams(obj?: any): Params {
  if (!obj) {
    return {} as Params;
  }
  const params: Params = {};
  Object.keys(obj).forEach((key) => {
    const value = obj[key];
    if (value !== undefined) {
      if (value instanceof Date) {
        params[key] = value.toISOString();
        return;
      }
      if (value instanceof Array) {
        params[key] = value.join(',');
        return;
      }
      params[key] = value;
    }
  });
  return removeEmptyParams(params);
}
