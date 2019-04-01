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
                if(resp.status == 200)
                    resolve(resp);
                else
                {
                    if(resp.status == 401)
                    {
                        console.log("Missing token");
                        appManager.renewToken()
                        .then(user => {
                            console.log("New token");
                            return fetch(url, {
                                method:  method,
                                body: body,
                                headers: {
                                    'Authorization': 'Bearer '+ user.access_token,
                                }
                            })
                        });
                    }
                    else
                        reject(resp);
                }
            })));
}


export default authFetch;