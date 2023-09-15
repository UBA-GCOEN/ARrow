import userVisitorModel from "../models/userVisitorModel.js"
import bcrypt from 'bcrypt'
import generateToken from "../middlewares/generateToken.js"

/**
 * Route: /userVisitor
 * Desc: to show or access user Visitor
 */
export const userVisitor = async (req, res) => {
    res.status(200).json({message:"Show user Visitor signin/signup page"})

}



/**
 * Route: /userVisitor/signup
 * Desc: Visitor user sign up
 */
export const signup = async (req, res) => {
       const { 
        name, 
        email, 
        password, 
        confirmPassword, 
        bio
      } = req.body

              
       //check if any field is not empty
       if (!name || !email || !password || !confirmPassword) {
        return res.status(404).json({
          success: false,
          message: "Please Fill all the Details.",
        });
      }
      
      //password and email constrains
      const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$%#^&*])(?=.*[0-9]).{8,}$/;

      const emailDomains = [
       "@gmail.com",
       "@yahoo.com",
       "@hotmail.com",
       "@aol.com",
       "@outlook.com",
       ];



       //check name length
       if (name.length < 2) {
         return res
           .status(404)
           .json({ message: "Name must be atleast 2 characters long." });
       }



       // check email format
       if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
         return res.status(404).json({
        message: "Please enter a valid email address",
      })};


      // check password format
       if (!passwordRegex.test(password)) {
         return res.status(404).json({
        message: "Password must be at least 8 characters long and include at least 1 uppercase letter, 1 lowercase letter, 1 symbol (@$%#^&*), and 1 number (0-9)",
        });
      }            
 
        
      // check password match
       if(password != confirmPassword){
         res.json({msg:"Password does not match"})
         }    
       


         /**
          * checking field types
          * to avoid sql attacks
          */
         if (typeof name !== "string") {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof email !== "string") {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof password !== "string" || typeof confirmPassword !== "string") {
          res.status(400).json({ status: "error" });
          return;
        }    
        
        if (typeof bio !== "string") {
          res.status(400).json({ status: "error" });
          return;
        }
        

       const oldUser = await userVisitorModel.findOne({ email });
       try{
        if(!oldUser){
 

          // hash password with bcrypt
           const hashedPassword = await bcrypt.hash(password, 12)
           
           // create userVisitor in database 
            const result = userVisitorModel.create({
                name,
                email,
                password: hashedPassword,
                bio
             });
    
             if(result){
                res.json({msg: "user Visitor added successfully"})
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
 * Route: /userVisitor/signin
 * Desc: user Visitor sign in
 */
export const signin = async (req, res) => {
      const {email, password} = req.body  
      
      const oldUser = await userVisitorModel.findOne({email})

      const SECRET = process.env.VISITOR_SECRET
      
      if(oldUser){
        
        const isPasswordCorrect = bcrypt.compare(oldUser.password, password)

        if(isPasswordCorrect){
          
          const token = generateToken(oldUser, SECRET);

          req.session.user = {
            token: token,
            user: oldUser
          }

          res.status(200).json({
            success: true,
            result: oldUser,
            token,
            csrfToken: req.csrfToken,
            msg: "Visitor is logged in successfully"
          });

        }
        else{
            res.json({ msg: "Incorrect password" })
        }
      }
      else{
        res.json({ msg:"User Visitor does not exist" })
      }
}






