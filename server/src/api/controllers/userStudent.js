import userStudentModel from "../models/userStudentModel.js"
import bcrypt from 'bcrypt'

/**
 * Route: /userStudent
 * Desc: to show or Access user Student
 */
export const userStudent = async (req, res) => {
    res.status(200).json({message:"Show user student signin/signup page"})

}


/**
 * Route: /userStudent/signup
 * Desc: Student user sign up
 */
export const signup = async (req, res) => {
       const { name, enrollNo, email, password, confirmPassword} = req.body
       

              
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

       const oldUser = await userStudentModel.findOne({ email });
      try{
        if(!oldUser){   
  

           // hash password with bcrypt
            const hashedPassword = await bcrypt.hash(password, 12)
           

           // create userstudent in database 
            const result = userStudentModel.create({
                name,
                enrollNo,
                email,
                password: hashedPassword,
             });
    
             if(result){
                res.json({msg: "user Student added successfully"})
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
 * Route: /userStudent/signin
 * Desc: user student sign in
 */
export const signin = async (req, res) => {
  const {email, password} = req.body  
  
  const oldUser = await userStudentModel.findOne({email})
  
  if(oldUser){

    const isPasswordCorrect = bcrypt.compare(oldUser.password, password)

    if(isPasswordCorrect){
        res.json({ msg: "student is logged in successfully" })
    }
    else{
        res.json({ msg: "Incorrect password" })
    }
  }
  else{
    res.json({msg:"User Student does not exist."})
  }
}