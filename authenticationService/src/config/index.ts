import { config } from 'dotenv';
config({ path: `.env.${process.env.NODE_ENV || 'development'}.local` });

export const CREDENTIALS = process.env.CREDENTIALS === 'true';
export const {
  NODE_ENV,
  PORT,
  SECRET_KEY,
  LOG_FORMAT,
  LOG_DIR,
  ORIGIN,
  YANDEX_CLIENT_ID,
  YANDEX_CLIENT_SECRET,
  DISCORD_SERVER_URL,
  SYNC_SERVICE_HOST,
} = process.env;
