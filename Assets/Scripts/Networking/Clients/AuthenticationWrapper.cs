using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState authState { get; private set;} = AuthState.NotAuthenticate;

    public static async Task<AuthState> DoAuth(int maxRetries = 5){
        if(authState == AuthState.Authenticated){ return authState; }
        if(authState == AuthState.Authenticating){
            Debug.LogWarning("Already authenticating !!!");
            return await Authenticating();
        }

        await SignInAnonymuslyAsync(maxRetries);
      
        return authState;
    }

    private static async Task<AuthState> Authenticating(){

        while(authState == AuthState.Authenticating || authState == AuthState.NotAuthenticate){
            await Task.Delay(200);
        }

        return authState;
    }

    private static async Task SignInAnonymuslyAsync(int maxRetries){

        authState = AuthState.Authenticating;
        int retries = 0;
        while(authState == AuthState.Authenticating && retries < maxRetries){
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if(AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized){
                    authState = AuthState.Authenticated;
                    break;
                }
            } catch(AuthenticationException ex){
                Debug.LogError(ex);
                authState = AuthState.Error;
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError(ex);
                authState = AuthState.Error;

            }
            retries++;
            await Task.Delay(1000);
        }

        if(authState != AuthState.Authenticated){
            Debug.LogWarning($"Player was not signed in successfully after {retries} retries");
            authState = AuthState.TimeOut;
        }
    }
    
}


public enum AuthState{
    NotAuthenticate,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
