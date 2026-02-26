import { Amplify } from 'aws-amplify';
import { cognitoUserPoolsTokenProvider } from 'aws-amplify/auth/cognito';
import AsyncStorage from '@react-native-async-storage/async-storage';

Amplify.configure({
  Auth: {
    Cognito: {
      userPoolId: process.env.EXPO_PUBLIC_COGNITO_USER_POOL_ID!,
      userPoolClientId: process.env.EXPO_PUBLIC_COGNITO_CLIENT_ID!,
      loginWith: {
        oauth: {
          domain: process.env.EXPO_PUBLIC_COGNITO_DOMAIN!,
          scopes: ['openid', 'email', 'profile'],
          redirectSignIn: [process.env.EXPO_PUBLIC_AUTH_REDIRECT_SIGN_IN!],
          redirectSignOut: [process.env.EXPO_PUBLIC_AUTH_REDIRECT_SIGN_OUT!],
          responseType: 'code',
        },
      },
    },
  },
});

cognitoUserPoolsTokenProvider.setKeyValueStorage(AsyncStorage);
