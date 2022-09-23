import axios from 'axios';
import { SYNC_SERVICE_HOST } from '@config';

export const updateOrCreateUser: (discordId: string, email: string) => Promise<boolean> = (discordId, email) => {
  console.log('DISCORDID:', discordId, 'EMAIL:', email);
  axios
    .post(`${SYNC_SERVICE_HOST}/discord/users`, {
      discordId: discordId,
      email: email,
    })
    .then(res => {
      console.log(`statusCode: ${res.status}`);
      console.log(res);
    })
    .catch(error => {
      console.error(error);
    });
  //ДОБАВИТЬ ЗДЕСЬ ВЫЗОВ AXIOS
  return Promise.resolve(true);
};
