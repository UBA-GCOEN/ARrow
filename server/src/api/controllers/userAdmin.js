import userAdminModel from "../models/userAdminModel.js"


/**
 * Route: /userAdmin
 * Desc: to show or access user Admin
 */
export const userAdmin = async (req, res) => {
    res.status(200).json({message:"Show user Admin signin/signup page"})

}


/**
 * Route: /userAdmin/signup
 * Desc: Admin user sign up
 */
export const signup = async (req, res) => {
       const { name, email, password} = req.body

       const oldUser = await userAdminModel.findOne({ email });
      try{
        if(!oldUser){
            const result = userAdminModel.create({
                name,
                email,
                password,
             });
    
             if(result){
                res.json({msg: "user Admin added successfully"})
             }
           }
           else{
            res.json({msg: "user already exist"})
           }
      }
      catch(err){
        console.log(err)
      }
     

}


/**
 * Route: /userAdmin/signin
 * Desc: user admin sign in
 */
export const signin = async (req, res) => {
      const {email, password} = req.body  
      
      const oldUser = await userAdminModel.findOne({email})
      
      if(oldUser){
        if(oldUser.password == password){
            res.json({ msg: "user is logged in successfully" })
        }
        else{
            res.json({ msg: "Incorrect password" })
        }
      }
}
