export const IDENTITY_CONFIG = {

    authority: process.env.REACT_APP_OIDC_AUTH_URL,
    client_id: process.env.REACT_APP_OIDC_CLIENT_ID,
    redirect_uri: `${window.location.origin}/signin-oidc`,
    silent_redirect_uri: `${window.location.origin}/silentrenew`,
    post_logout_redirect_uri: window.location.origin,
    response_type: 'id_token token',
    scope: 'openid'


 }

