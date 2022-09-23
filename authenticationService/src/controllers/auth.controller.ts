import { updateOrCreateUser } from '@/services/auth.service';
import { DISCORD_SERVER_URL } from '@config';

export const authenticate = (req, res) => {};

export const handleSuccessfullAuthentication = async (req, res) => {
  const discordId = req.query.state;
  await updateOrCreateUser(discordId, req.user.username).then(result => res.redirect(DISCORD_SERVER_URL));
};
