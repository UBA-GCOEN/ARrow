import '../middlewares/passportConfig.js'
import  passport  from 'passport'
import generateToken from "../middlewares/generateToken.js";



/**
 * Route: /auth/google
 * Desc:  Open google consent screen
 */
export const authGoogle = passport.authenticate('google', { scope: [ 'email', 'profile' ]})


/**
 * Route: /auth/google/callback
 * Desc: handle callback from google
 */
export const callbackGoogle = passport.authenticate('google', { successRedirect: '/auth/protected', failureRedirect: '/auth/failed' })




/**
 * Route /protected
 * desc: reditrection after successfull 
 *       google auth with userdata in req
 */
export const  authenticated = (req, res)=>{
  // let name = req.user.displayName

  const SECRET = process.env.USER_SECRET
  const token = generateToken(req.user, SECRET);

    req.session.user = {
      token: token,
      user: req.user
    }

    res.status(200).json({
      success: true,
      user: req.user,
      token: token
    })
}


/**
 * Route: /failed
 * Desc: Redirection if google authentication failed
 */
export const failed = (req, res)=>{
  res.status(401).send("google authentication failed")
}