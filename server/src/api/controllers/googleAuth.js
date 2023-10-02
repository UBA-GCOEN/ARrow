import '../middlewares/passportConfig.js'
import  passport  from 'passport'



/**
 * Route: /auth/google
 * Desc:  Open google consent screen
 */
export const authGoogle = (req, res) => {
 
  passport.authenticate('google', { scope: ['profile', 'email'] })
}


/**
 * Route: /auth/google/callback
 * Desc: handle callback from google
 */
export const callbackGoogle = (req, res) => {
    passport.authenticate('google', { successRedirect: '/auth/protected', failureRedirect: '/auth/failed' })
}