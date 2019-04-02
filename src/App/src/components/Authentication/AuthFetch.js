import { AuthManager } from "./AuthManager"
const appManager = new AuthManager();

function authFetch(url, method, body){
    return appManager.getUser()
        .then(user => 
            fetch(url, {
                method:  method,
                body: body,
                headers: {
                    'Authorization': 'Bearer '+ user.access_token,
                }
            })
            .then(resp => new Promise((resolve, reject)=>{
                if(resp.status == 401) {
                    appManager.renewToken()
                    .then(user => {
                        resolve(
                        fetch(url, {
                            method:  method,
                            body: body,
                            headers: {
                                'Authorization': 'Bearer '+ user.access_token,
                            }
                        }));
                    });
                } 
                else {
                    resolve(resp);
                }
            })
            ))}
            

export default authFetch;